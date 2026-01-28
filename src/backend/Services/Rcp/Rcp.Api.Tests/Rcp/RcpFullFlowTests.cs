using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Bps.Enum.Rcp;
using App.Shared.Tests;
using Rcp.Api.Features.TimeEntry.Model;
using Rcp.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Rcp.Api.Tests.Rcp;

/// <summary>
/// Test pelnego cyklu zycia wpisu RCP:
/// Draft -> Submitted -> Approved -> Settlement
/// oraz scenariusze zwrotu do poprawy
/// </summary>
[Collection(RcpDatabaseCollection.Name)]
public class RcpFullFlowTests : IDisposable
{
    private readonly RcpMySqlTestContainer _container;
    private readonly RcpApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public RcpFullFlowTests(
        RcpMySqlTestContainer container,
        ITestOutputHelper output)
    {
        _container = container;
        _output = output;
        _factory = new RcpApplicationFactory(container.ConnectionString, _ =>
        {
            _.Roles = [nameof(AppRole.Root)];
            _.FirstName = "Test";
            _.LastName = "TestLast";
        });
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    /// <summary>
    /// Test pelnego flow: pracownik wypelnia godziny, wysyla do zatwierdzenia,
    /// przelozony zatwierdza i przekazuje do rozliczenia
    /// </summary>
    [Fact]
    public async Task FullFlow_HappyPath_DraftToSettlement()
    {
        // === KROK 1: Pracownik wypelnia godziny za tydzien pracy ===
        var entries = new[]
        {
            new { Day = 2, Hours = 8, Minutes = 0 },
            new { Day = 3, Hours = 8, Minutes = 30 },
            new { Day = 4, Hours = 7, Minutes = 45 },
            new { Day = 5, Hours = 8, Minutes = 0 },
            new { Day = 6, Hours = 9, Minutes = 15 }
        };

        foreach (var e in entries)
        {
            var request = new
            {
                Year = 2021,
                Month = 3,
                WorkDate = $"2021-03-{e.Day:D2}",
                Hours = e.Hours,
                Minutes = e.Minutes,
                StartTime = "08:00"
            };
            var response = await _client.PostAsJsonAsync("/api/entry", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Sprawdz podsumowanie
        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 3 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        Assert.Equal(RcpTimeEntryStatus.Draft, monthly!.Status);
        Assert.Equal(5, monthly.Days.Count);
        // 8:00 + 8:30 + 7:45 + 8:00 + 9:15 = 41:30 = 2490 min
        Assert.Equal(2490, monthly.TotalMinutes);
        Assert.Equal("41:30", monthly.TotalFormatted);
        Assert.Equal(41.5m, monthly.TotalDecimalHours);

        // === KROK 2: Pracownik wysyla wpis do zatwierdzenia ===
        var submitResponse = await _client.PostAsJsonAsync(
            "/api/my/submit",
            new { Year = 2021, Month = 3, Comment = "Prosze o zatwierdzenie godzin za marzec" });
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);

        // Sprawdz status
        monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 3 });
        monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Submitted, monthly!.Status);
        Assert.NotNull(monthly.SubmittedAt);

        // === KROK 3: Przelozony widzi wpis na liscie ===
        var supervisorListResponse = await _client.PostAsJsonAsync("/api/supervisor/period", new { Year = 2021, Month = 3 });
        var periodSummary = await supervisorListResponse.ReadWithJson<PeriodSummaryResponse>(_output);
        Assert.Contains(periodSummary!.Entries, e => e.Status == RcpTimeEntryStatus.Submitted);

        // === KROK 4: Przelozony zatwierdza wpis ===
        var approveResponse = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly.Gid}/approve",
            new { Comment = "Zatwierdzam godziny" });
        Assert.Equal(HttpStatusCode.OK, approveResponse.StatusCode);

        // Sprawdz status
        var entryResponse = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = monthly.Gid });
        var entry = await entryResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Approved, entry!.Status);
        Assert.NotNull(entry.ApprovedAt);

        // === KROK 5: Przelozony przekazuje do rozliczenia ===
        var toSettlementResponse = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly.Gid}/to-settlement",
            new { Comment = "Przekazuje do rozliczenia" });
        Assert.Equal(HttpStatusCode.OK, toSettlementResponse.StatusCode);

        // Sprawdz status
        entryResponse = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = monthly.Gid });
        entry = await entryResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Settlement, entry!.Status);

        // === KROK 6: Kadry/Przelozony widzi wpis na liscie do rozliczenia ===
        var payrollListResponse = await _client.PostAsJsonAsync("/api/payroll/period", new { Year = 2021, Month = 3 });
        var payrollSummary = await payrollListResponse.ReadWithJson<PeriodSummaryResponse>(_output);
        Assert.Contains(payrollSummary!.Entries, e => e.Status == RcpTimeEntryStatus.Settlement);

        // === KROK 7: Eksport do Excela ===
        var exportResponse = await _client.PostAsJsonAsync("/api/payroll/export", new { Year = 2021, Month = 3 });
        Assert.Equal(HttpStatusCode.OK, exportResponse.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            exportResponse.Content.Headers.ContentType?.MediaType);

        var excelBytes = await exportResponse.Content.ReadAsByteArrayAsync();
        Assert.True(excelBytes.Length > 0);
    }

    /// <summary>
    /// Test scenariusza z odrzuceniem i ponownym wyslaniem
    /// </summary>
    [Fact]
    public async Task Flow_WithRejection_DraftToSubmittedToCorrection()
    {
        // === KROK 1: Pracownik wypelnia i wysyla ===
        var request = new
        {
            Year = 2021,
            Month = 4,
            WorkDate = "2021-04-05",
            Hours = 12,
            Minutes = 0,
            StartTime = "08:00"
        };
        await _client.PostAsJsonAsync("/api/entry", request);
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2021, Month = 4 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 4 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // === KROK 2: Przelozony odrzuca wpis ===
        var rejectResponse = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly!.Gid}/reject",
            new { Comment = "12 godzin dziennie? Prosze o weryfikacje." });
        Assert.Equal(HttpStatusCode.OK, rejectResponse.StatusCode);

        // Sprawdz status - powinien byc Correction
        monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 4 });
        monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Correction, monthly!.Status);

        // === KROK 3: Pracownik moze teraz edytowac wpis ===
        var updateRequest = new
        {
            Year = 2021,
            Month = 4,
            WorkDate = "2021-04-05",
            Hours = 8,
            Minutes = 0,
            StartTime = "08:00",
            Notes = "Poprawione po uwagach przelozonego"
        };
        var updateResponse = await _client.PostAsJsonAsync("/api/entry", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // === KROK 4: Pracownik ponownie wysyla ===
        var resubmitResponse = await _client.PostAsJsonAsync(
            "/api/my/submit",
            new { Year = 2021, Month = 4, Comment = "Poprawilem wpis zgodnie z uwagami" });
        Assert.Equal(HttpStatusCode.OK, resubmitResponse.StatusCode);

        monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 4 });
        monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Submitted, monthly!.Status);

        // === KROK 5: Teraz przelozony zatwierdza ===
        var approveResponse = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly.Gid}/approve",
            new { });
        Assert.Equal(HttpStatusCode.OK, approveResponse.StatusCode);
    }

    /// <summary>
    /// Test scenariusza z cofnieciem do poprawy po zatwierdzeniu
    /// </summary>
    [Fact]
    public async Task Flow_ReturnFromApproved_ToCorrection()
    {
        // Setup: Draft -> Submitted -> Approved
        await _client.PostAsJsonAsync("/api/entry", new
        {
            Year = 2021,
            Month = 5,
            WorkDate = "2021-05-10",
            Hours = 8,
            Minutes = 0,
            StartTime = "08:00"
        });
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2021, Month = 5 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 5 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        await _client.PostAsJsonAsync($"/api/supervisor/{monthly!.Gid}/approve", new { });

        // === Kadry/Przelozony zwraca do poprawy ===
        var returnResponse = await _client.PostAsJsonAsync(
            "/api/payroll/return",
            new { Gid = monthly.Gid, Comment = "Brak podpisu na delegacji - prosze uzupelnic" });
        Assert.Equal(HttpStatusCode.OK, returnResponse.StatusCode);

        // Sprawdz status
        monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 5 });
        monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Correction, monthly!.Status);
    }

    /// <summary>
    /// Test systemu komentarzy podczas flow
    /// </summary>
    [Fact]
    public async Task Flow_Comments_ArePreserved()
    {
        // Setup wpisu
        await _client.PostAsJsonAsync("/api/entry", new
        {
            Year = 2021,
            Month = 6,
            WorkDate = "2021-06-15",
            Hours = 8,
            Minutes = 0,
            StartTime = "08:00"
        });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 6 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Dodaj komentarz jako pracownik
        await _client.PostAsJsonAsync($"/api/{monthly!.Gid}/comments",
            new { Content = "Pierwszy dzien po urlopie" });

        // Wyslij do zatwierdzenia z komentarzem
        await _client.PostAsJsonAsync("/api/my/submit",
            new { Year = 2021, Month = 6, Comment = "Prosze o zatwierdzenie" });

        // Odrzuc z komentarzem
        await _client.PostAsJsonAsync($"/api/supervisor/{monthly.Gid}/reject",
            new { Comment = "Prosze dodac info o delegacji" });

        // Dodaj kolejny komentarz
        await _client.PostAsJsonAsync($"/api/{monthly.Gid}/comments",
            new { Content = "Dodaje informacje o delegacji w notatkach" });

        // Pobierz wszystkie komentarze
        var commentsResponse = await _client.PostAsJsonAsync("/api/comments/list", new { Gid = monthly.Gid });
        var comments = await commentsResponse.ReadWithJson<List<CommentResponse>>(_output);

        // Powinny byc co najmniej 4 komentarze (2 manualne + 2 automatyczne przy zmianie statusu)
        Assert.True(comments!.Count >= 4);
    }

    /// <summary>
    /// Test walidacji - nie mozna edytowac wpisu po wyslaniu
    /// </summary>
    [Fact]
    public async Task Validation_CannotEditAfterSubmit()
    {
        // Setup i wyslanie
        await _client.PostAsJsonAsync("/api/entry", new
        {
            Year = 2021,
            Month = 7,
            WorkDate = "2021-07-01",
            Hours = 8,
            Minutes = 0,
            StartTime = "08:00"
        });
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2021, Month = 7 });

        // Proba edycji po wyslaniu
        var editResponse = await _client.PostAsJsonAsync("/api/entry", new
        {
            Year = 2021,
            Month = 7,
            WorkDate = "2021-07-01",
            Hours = 10,
            Minutes = 0,
            StartTime = "07:00"
        });

        Assert.Equal(HttpStatusCode.BadRequest, editResponse.StatusCode);
    }

    /// <summary>
    /// Test ze po zwrocie do Correction mozna znow edytowac
    /// </summary>
    [Fact]
    public async Task CanEdit_AfterReturnToCorrection()
    {
        // Setup: Draft -> Submitted -> Rejected (Correction)
        await _client.PostAsJsonAsync("/api/entry", new
        {
            Year = 2021,
            Month = 8,
            WorkDate = "2021-08-05",
            Hours = 8,
            Minutes = 0,
            StartTime = "08:00"
        });
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2021, Month = 8 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2021, Month = 8 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        await _client.PostAsJsonAsync($"/api/supervisor/{monthly!.Gid}/reject", new { });

        // Teraz powinnismy moc edytowac
        var editResponse = await _client.PostAsJsonAsync("/api/entry", new
        {
            Year = 2021,
            Month = 8,
            WorkDate = "2021-08-05",
            Hours = 9,
            Minutes = 30,
            StartTime = "07:30",
            Notes = "Poprawione po odrzuceniu"
        });

        Assert.Equal(HttpStatusCode.OK, editResponse.StatusCode);
        var result = await editResponse.ReadWithJson<DayEntryResponse>(_output);
        Assert.Equal(570, result!.WorkMinutes); // 9:30 = 570 min
    }
}
