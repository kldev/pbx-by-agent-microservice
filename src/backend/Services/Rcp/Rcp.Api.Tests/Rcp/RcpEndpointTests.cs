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

[Collection(RcpDatabaseCollection.Name)]
public class RcpEndpointTests : IDisposable
{
    private readonly RcpMySqlTestContainer _container;
    private readonly RcpApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public RcpEndpointTests(
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

    #region Helper Methods

    private async Task<DayEntryResponse> CreateTestDayEntryAsync(
        int year,
        int month,
        int day,
        int hours = 8,
        int minutes = 0,
        string? startTime = null)
    {
        var request = new
        {
            Year = year,
            Month = month,
            WorkDate = $"{year:D4}-{month:D2}-{day:D2}",
            Hours = hours,
            Minutes = minutes,
            StartTime = startTime ?? "08:00",
            Notes = "Test entry"
        };

        var response = await _client.PostAsJsonAsync("/api/entry", request);
        var result = await response.ReadWithJson<DayEntryResponse>(_output);
        return result!;
    }

    #endregion

    #region POST /api/entry - Save Day Entry

    [Fact]
    public async Task SaveDayEntry_ReturnsOk_WhenValid()
    {
        // Arrange - use a unique year/month
        var request = new
        {
            Year = 2023,
            Month = 1,
            WorkDate = "2023-01-15",
            Hours = 8,
            Minutes = 30,
            StartTime = "08:00",
            Notes = "Test work day"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/entry", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<DayEntryResponse>(_output);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Gid);
        Assert.Equal(510, result.WorkMinutes); // 8*60 + 30
        Assert.Equal("8:30", result.TimeFormatted);
    }

    [Fact]
    public async Task SaveDayEntry_ReturnsBadRequest_WhenFutureMonth()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddMonths(1);
        var request = new
        {
            Year = futureDate.Year,
            Month = futureDate.Month,
            WorkDate = futureDate.ToString("yyyy-MM-dd"),
            Hours = 8,
            Minutes = 0,
            StartTime = "08:00"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/entry", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SaveDayEntry_ReturnsBadRequest_WhenInvalidMinutes()
    {
        // Arrange - minutes not multiple of 5
        var request = new
        {
            Year = 2023,
            Month = 2,
            WorkDate = "2023-02-10",
            Hours = 8,
            Minutes = 17, // Not a multiple of 5
            StartTime = "08:00"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/entry", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SaveDayEntry_ReturnsBadRequest_WhenZeroTime()
    {
        // Arrange
        var request = new
        {
            Year = 2023,
            Month = 3,
            WorkDate = "2023-03-05",
            Hours = 0,
            Minutes = 0,
            StartTime = "08:00"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/entry", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SaveDayEntry_UpdatesExisting_WhenSameDate()
    {
        // Arrange - Create initial entry
        await CreateTestDayEntryAsync(2023, 4, 10, 8, 0);

        // Act - Update with new hours
        var updateRequest = new
        {
            Year = 2023,
            Month = 4,
            WorkDate = "2023-04-10",
            Hours = 10,
            Minutes = 30,
            StartTime = "07:00",
            Notes = "Updated entry"
        };
        var response = await _client.PostAsJsonAsync("/api/entry", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<DayEntryResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(630, result.WorkMinutes); // 10*60 + 30
    }

    #endregion

    #region DELETE /api/entry - Delete Day Entry

    [Fact]
    public async Task DeleteDayEntry_ReturnsOk_WhenExists()
    {
        // Arrange
        await CreateTestDayEntryAsync(2023, 5, 15);

        // Act
        var response = await _client.DeleteAsync("/api/entry/2023/5/2023-05-15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDayEntry_ReturnsBadRequest_WhenInvalidDate()
    {
        // Act
        var response = await _client.DeleteAsync("/api/entry/2023/6/invalid-date");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region POST /api/my - Get My Monthly Entry

    [Fact]
    public async Task GetMyMonthlyEntry_ReturnsEmptyEntry_WhenNoData()
    {
        // Arrange - use a month with no data
        var emptyYear = 2020;
        var emptyMonth = 1;

        // Act
        var response = await _client.PostAsJsonAsync("/api/my", new { Year = emptyYear, Month = emptyMonth });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(RcpTimeEntryStatus.Draft, result.Status);
        Assert.Equal(0, result.TotalMinutes);
    }

    [Fact]
    public async Task GetMyMonthlyEntry_ReturnsEntryWithDays_WhenDataExists()
    {
        // Arrange - use unique month
        await CreateTestDayEntryAsync(2023, 7, 10, 8, 0);

        // Act
        var response = await _client.PostAsJsonAsync("/api/my", new { Year = 2023, Month = 7 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Gid);
        Assert.Equal(RcpTimeEntryStatus.Draft, result.Status);
        Assert.Equal(480, result.TotalMinutes);
        Assert.NotEmpty(result.Days);
    }

    #endregion

    #region POST /api/my/submit - Submit Entry

    [Fact]
    public async Task SubmitEntry_ReturnsOk_WhenValid()
    {
        // Arrange - Create entry first
        await CreateTestDayEntryAsync(2022, 1, 10, 8, 0);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/my/submit",
            new { Year = 2022, Month = 1, Comment = "Prosze o zatwierdzenie" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify status changed
        var getResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 1 });
        var entry = await getResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Submitted, entry!.Status);
    }

    [Fact]
    public async Task SubmitEntry_ReturnsBadRequest_WhenEmpty()
    {
        // Arrange - No entries for this month
        var emptyYear = 2019;
        var emptyMonth = 2;

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/my/submit",
            new { Year = emptyYear, Month = emptyMonth });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region POST /api/supervisor - Supervisor Endpoints

    [Fact]
    public async Task GetSupervisorPeriodEntries_ReturnsOk()
    {
        // Arrange
        await CreateTestDayEntryAsync(2022, 2, 5);

        // Act
        var response = await _client.PostAsJsonAsync("/api/supervisor/period", new { Year = 2022, Month = 2 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<PeriodSummaryResponse>(_output);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetEntryByGid_ReturnsOk_WhenExists()
    {
        // Arrange
        await CreateTestDayEntryAsync(2022, 3, 12);

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 3 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Act
        var response = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = monthly!.Gid });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal(monthly.Gid, result.Gid);
    }

    [Fact]
    public async Task GetEntryByGid_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = Guid.NewGuid().ToString() });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region POST /api/supervisor - Approve/Reject

    [Fact]
    public async Task ApproveEntry_ReturnsOk_WhenSubmitted()
    {
        // Arrange - Create and submit entry
        await CreateTestDayEntryAsync(2022, 4, 8, 8, 0);
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2022, Month = 4 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 4 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly!.Gid}/approve",
            new { Comment = "Zatwierdzam" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify status
        var getResponse = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = monthly.Gid });
        var entry = await getResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Approved, entry!.Status);
    }

    [Fact]
    public async Task RejectEntry_ReturnsOk_WhenSubmitted()
    {
        // Arrange - Create and submit entry
        await CreateTestDayEntryAsync(2022, 5, 10, 8, 0);
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2022, Month = 5 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 5 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly!.Gid}/reject",
            new { Comment = "Prosze poprawic godziny" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify status
        var getResponse = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = monthly.Gid });
        var entry = await getResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Correction, entry!.Status);
    }

    [Fact]
    public async Task ApproveEntry_ReturnsBadRequest_WhenNotSubmitted()
    {
        // Arrange - Create entry but don't submit
        await CreateTestDayEntryAsync(2022, 6, 15, 8, 0);

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 6 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly!.Gid}/approve",
            new { });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region POST /api/supervisor/{gid}/to-settlement

    [Fact]
    public async Task ToSettlement_ReturnsOk_WhenApproved()
    {
        // Arrange - Create, submit, and approve
        await CreateTestDayEntryAsync(2022, 7, 20, 8, 0);
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2022, Month = 7 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 7 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        await _client.PostAsJsonAsync($"/api/supervisor/{monthly!.Gid}/approve", new { });

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/supervisor/{monthly.Gid}/to-settlement",
            new { Comment = "Do rozliczenia" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify status
        var getResponse = await _client.PostAsJsonAsync("/api/supervisor/entry", new { Gid = monthly.Gid });
        var entry = await getResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Settlement, entry!.Status);
    }

    #endregion

    #region POST /api/payroll - Payroll Endpoints

    [Fact]
    public async Task GetPayrollEntries_ReturnsOk()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/payroll/period", new { Year = 2021, Month = 1 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<PeriodSummaryResponse>(_output);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ReturnForCorrection_ReturnsOk_WhenApproved()
    {
        // Arrange - Create full flow to Approved
        await CreateTestDayEntryAsync(2022, 8, 10, 8, 0);
        await _client.PostAsJsonAsync("/api/my/submit", new { Year = 2022, Month = 8 });

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 8 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        await _client.PostAsJsonAsync($"/api/supervisor/{monthly!.Gid}/approve", new { });

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/payroll/return",
            new { Gid = monthly.Gid, Comment = "Prosze zweryfikowac wpis" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify status
        var getResponse = await _client.PostAsJsonAsync("/api/payroll/entry", new { Gid = monthly.Gid });
        var entry = await getResponse.ReadWithJson<MonthlyEntryResponse>(_output);
        Assert.Equal(RcpTimeEntryStatus.Correction, entry!.Status);
    }

    #endregion

    #region POST /api/payroll/export - Excel Export

    [Fact]
    public async Task ExportToExcel_ReturnsFile()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/payroll/export", new { Year = 2021, Month = 2 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            response.Content.Headers.ContentType?.MediaType);
    }

    #endregion

    #region Comments

    [Fact]
    public async Task GetComments_ReturnsEmptyList_WhenNoComments()
    {
        // Arrange
        await CreateTestDayEntryAsync(2022, 9, 5);

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 9 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Act
        var response = await _client.PostAsJsonAsync("/api/comments/list", new { Gid = monthly!.Gid });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<List<CommentResponse>>(_output);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddComment_ReturnsOk()
    {
        // Arrange
        await CreateTestDayEntryAsync(2022, 10, 12);

        var monthlyResponse = await _client.PostAsJsonAsync("/api/my", new { Year = 2022, Month = 10 });
        var monthly = await monthlyResponse.ReadWithJson<MonthlyEntryResponse>(_output);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/{monthly!.Gid}/comments",
            new { Content = "To jest komentarz testowy" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<CommentResponse>(_output);
        Assert.NotNull(result);
        Assert.Equal("To jest komentarz testowy", result.Content);
    }

    #endregion
}
