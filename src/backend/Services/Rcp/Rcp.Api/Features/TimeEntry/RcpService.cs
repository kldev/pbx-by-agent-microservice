using App.Bps.Enum;
using App.Bps.Enum.Rcp;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Rcp.Api.Features.TimeEntry.Model;
using Rcp.Api.Infrastructure;
using Rcp.Data.Entities;

namespace Rcp.Api.Features.TimeEntry;

public class RcpService : IRcpService
{
    private readonly IRcpDataHandler _dataHandler;
    private readonly IDataSourceClient _dataSourceClient;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RcpService(IRcpDataHandler dataHandler, IDataSourceClient dataSourceClient)
    {
        _dataHandler = dataHandler;
        _dataSourceClient = dataSourceClient;
    }

    /// <summary>
    /// Check if user has admin/root bypass (sees all entries)
    /// </summary>
    private static bool HasAdminBypass(PortalAuthInfo auth) =>
        auth.HasRole(AppRole.Root);

    private static Result<T> ValidationFailure<T>(string message) =>
        Result<T>.Failure(new ValidationError([message]));

    private static Result<T> NotFound<T>(string message) =>
        Result<T>.Failure(new NotFoundError("not_found", message));

    #region Own Entries

    public async Task<Result<DayEntryResponse>> SaveDayEntryAsync(PortalAuthInfo auth, SaveDayEntryRequest request)
    {
        // Parse WorkDate (format: "yyyy-MM-dd")
        if (!DateOnly.TryParse(request.WorkDate, out var workDate))
            return ValidationFailure<DayEntryResponse>("Nieprawidłowy format daty (oczekiwany: yyyy-MM-dd)");

        // Parse StartTime (format: "HH:mm")
        if (!TimeOnly.TryParse(request.StartTime, out var startTime))
            return ValidationFailure<DayEntryResponse>("Nieprawidłowy format godziny rozpoczęcia (oczekiwany: HH:mm)");

        // Validation: No future months
        var now = DateTime.UtcNow;
        if (request.Year > now.Year || (request.Year == now.Year && request.Month > now.Month))
            return ValidationFailure<DayEntryResponse>("Nie można wprowadzać godzin dla przyszłych miesięcy");

        // Validation: WorkDate must be in Year/Month
        if (workDate.Year != request.Year || workDate.Month != request.Month)
            return ValidationFailure<DayEntryResponse>("Data musi należeć do podanego miesiąca");

        // Validation: Minutes must be multiple of 5
        if (request.Minutes < 0 || request.Minutes >= 60 || request.Minutes % 5 != 0)
            return ValidationFailure<DayEntryResponse>("Minuty muszą być wielokrotnością 5 (0, 5, 10, ..., 55)");

        // Validation: Hours in valid range
        if (request.Hours < 0 || request.Hours > 24)
            return ValidationFailure<DayEntryResponse>("Godziny muszą być między 0 a 24");

        var workMinutes = request.Hours * 60 + request.Minutes;
        if (workMinutes <= 0)
            return ValidationFailure<DayEntryResponse>("Czas pracy musi być większy niż 0");

        // Get or create period
        var period = await _dataHandler.GetOrCreatePeriodAsync(request.Year, request.Month);

        // Get or create monthly entry
        var monthlyEntry = await _dataHandler.GetMonthlyEntryAsync(period.Id, auth.UserId);
        if (monthlyEntry == null)
        {
            monthlyEntry = new RcpMonthlyEntry
            {
                Gid = Guid.NewGuid().ToString(),
                RcpPeriodId = period.Id,
                UserId = auth.UserId,
                UserGid = auth.Gid,
                Status = RcpTimeEntryStatus.Draft,
                EmployeeFullName = auth.FullName
            };
            await _dataHandler.CreateMonthlyEntryAsync(monthlyEntry);
        }

        // Check if editable
        if (monthlyEntry.Status != RcpTimeEntryStatus.Draft && monthlyEntry.Status != RcpTimeEntryStatus.Correction)
            return ValidationFailure<DayEntryResponse>("Wpis nie jest w trybie edycji");

        // Convert to DateTime/TimeSpan for MySQL
        var workDateTime = workDate.ToDateTime(TimeOnly.MinValue);
        var startTimeSpan = startTime.ToTimeSpan();
        var endTimeSpan = startTimeSpan.Add(TimeSpan.FromMinutes(workMinutes));

        // Upsert day entry
        var dayEntry = await _dataHandler.GetDayEntryAsync(monthlyEntry.Id, workDateTime);
        if (dayEntry == null)
        {
            dayEntry = new RcpDayEntry
            {
                Gid = Guid.NewGuid().ToString(),
                MonthlyEntryId = monthlyEntry.Id,
                WorkDate = workDateTime,
                StartTime = startTimeSpan,
                EndTime = endTimeSpan,
                WorkMinutes = workMinutes,
                Notes = request.Notes
            };
            await _dataHandler.CreateDayEntryAsync(dayEntry);
        }
        else
        {
            dayEntry.StartTime = startTimeSpan;
            dayEntry.EndTime = endTimeSpan;
            dayEntry.WorkMinutes = workMinutes;
            dayEntry.Notes = request.Notes;
            await _dataHandler.UpdateDayEntryAsync(dayEntry);
        }

        // Recalculate total
        monthlyEntry.TotalMinutes = await _dataHandler.CalculateTotalMinutesAsync(monthlyEntry.Id);
        await _dataHandler.UpdateMonthlyEntryAsync(monthlyEntry);

        return Result<DayEntryResponse>.Success(MapToDayResponse(dayEntry));
    }

    public async Task<Result<bool>> DeleteDayEntryAsync(PortalAuthInfo auth, int year, int month, DateTime workDate)
    {
        var period = await _dataHandler.GetPeriodAsync(year, month);
        if (period == null)
            return ValidationFailure<bool>("Brak wpisu dla tego miesiąca");

        var monthlyEntry = await _dataHandler.GetMonthlyEntryAsync(period.Id, auth.UserId);
        if (monthlyEntry == null)
            return ValidationFailure<bool>("Brak wpisu dla tego miesiąca");

        if (monthlyEntry.Status != RcpTimeEntryStatus.Draft && monthlyEntry.Status != RcpTimeEntryStatus.Correction)
            return ValidationFailure<bool>("Wpis nie jest w trybie edycji");

        var dayEntry = await _dataHandler.GetDayEntryAsync(monthlyEntry.Id, workDate);
        if (dayEntry == null)
            return ValidationFailure<bool>("Brak wpisu dla tego dnia");

        await _dataHandler.DeleteDayEntryAsync(dayEntry);

        // Recalculate total
        monthlyEntry.TotalMinutes = await _dataHandler.CalculateTotalMinutesAsync(monthlyEntry.Id);
        await _dataHandler.UpdateMonthlyEntryAsync(monthlyEntry);

        return Result<bool>.Success(true);
    }

    public async Task<Result<MonthlyEntryResponse>> GetMyMonthlyEntryAsync(PortalAuthInfo auth, int year, int month)
    {
        var period = await _dataHandler.GetPeriodAsync(year, month);

        RcpMonthlyEntry? monthlyEntry = null;
        if (period != null)
        {
            monthlyEntry = await _dataHandler.GetMonthlyEntryAsync(period.Id, auth.UserId);
        }

        // Return empty entry if not exists
        if (monthlyEntry == null)
        {
            return Result<MonthlyEntryResponse>.Success(new MonthlyEntryResponse(
                Gid: string.Empty,
                Year: year,
                Month: month,
                Status: RcpTimeEntryStatus.Draft,
                TotalMinutes: 0,
                TotalFormatted: "0:00",
                TotalDecimalHours: 0,
                EmployeeFullName: auth.FullName,
                UserGid: auth.Gid,
                SubmittedAt: null,
                ApprovedAt: null,
                ApprovedByFullName: null,
                Days: [],
                Comments: []
            ));
        }

        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(monthlyEntry, year, month));
    }

    public async Task<Result<MonthlyEntryResponse>> SubmitEntryAsync(PortalAuthInfo auth, SubmitRequest request)
    {
        var period = await _dataHandler.GetPeriodAsync(request.Year, request.Month);
        if (period == null)
            return ValidationFailure<MonthlyEntryResponse>("Brak wpisu dla tego miesiąca");

        var monthlyEntry = await _dataHandler.GetMonthlyEntryAsync(period.Id, auth.UserId);
        if (monthlyEntry == null || monthlyEntry.TotalMinutes == 0)
            return ValidationFailure<MonthlyEntryResponse>("Brak wpisów do wysłania");

        if (monthlyEntry.Status != RcpTimeEntryStatus.Draft && monthlyEntry.Status != RcpTimeEntryStatus.Correction)
            return ValidationFailure<MonthlyEntryResponse>("Wpis nie jest w trybie edycji");

        monthlyEntry.Status = RcpTimeEntryStatus.Submitted;
        monthlyEntry.SubmittedAt = DateTime.UtcNow;
        monthlyEntry.StatusChangedAt = DateTime.UtcNow;
        monthlyEntry.StatusChangedByUserId = auth.UserId;
        monthlyEntry.StatusChangedByFullName = auth.FullName;

        await _dataHandler.UpdateMonthlyEntryAsync(monthlyEntry);

        // Add comment if provided
        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            await AddCommentInternalAsync(monthlyEntry.Id, auth, request.Comment, "Pracownik");
        }

        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(monthlyEntry, request.Year, request.Month));
    }

    #endregion

    #region Supervisor

    public async Task<Result<PeriodSummaryResponse>> GetSupervisorPeriodEntriesAsync(PortalAuthInfo auth, int year, int month)
    {
        var period = await _dataHandler.GetPeriodAsync(year, month);
        if (period == null)
        {
            return Result<PeriodSummaryResponse>.Success(new PeriodSummaryResponse(year, month, []));
        }

        var allEntries = await _dataHandler.GetMonthlyEntriesForPeriodAsync(period.Id);

        IEnumerable<RcpMonthlyEntry> filteredEntries;

        // Root/Admin widzi wszystko
        if (HasAdminBypass(auth))
        {
            // For admin view: show only Submitted (waiting for approval)
            filteredEntries = allEntries.Where(e => e.Status == RcpTimeEntryStatus.Submitted);
        }
        else
        {
            // Supervisor widzi tylko swoich podwładnych ze statusem Submitted
            var subordinateIds = await _dataSourceClient.GetSubordinateIdsAsync(auth.UserId);
            filteredEntries = allEntries
                .Where(e => subordinateIds.Contains(e.UserId) && e.Status == RcpTimeEntryStatus.Submitted);
        }

        var summaries = filteredEntries.Select(e => new MonthlyEntrySummaryResponse(
            e.Gid,
            e.EmployeeFullName,
            e.UserGid,
            e.Status,
            e.TotalMinutes,
            FormatMinutes(e.TotalMinutes),
            e.SubmittedAt
        )).ToList();

        return Result<PeriodSummaryResponse>.Success(new PeriodSummaryResponse(year, month, summaries));
    }

    public async Task<Result<MonthlyEntryResponse>> GetEntryByGidAsync(PortalAuthInfo auth, string gid)
    {
        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(gid);
        if (entry == null)
            return NotFound<MonthlyEntryResponse>("Wpis nie został znaleziony");

        // Validate access for non-admin users
        if (!HasAdminBypass(auth))
        {
            var subordinateIds = await _dataSourceClient.GetSubordinateIdsAsync(auth.UserId);
            if (!subordinateIds.Contains(entry.UserId))
                return Result<MonthlyEntryResponse>.Failure(new ForbiddenError(
                    "access_denied", "Nie masz uprawnień do tego wpisu"));
        }

        var year = entry.RcpPeriod?.Year ?? 0;
        var month = entry.RcpPeriod?.Month ?? 0;

        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(entry, year, month));
    }

    public async Task<Result<MonthlyEntryResponse>> ApproveEntryAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request)
    {
        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(gid);
        if (entry == null)
            return NotFound<MonthlyEntryResponse>("Wpis nie został znaleziony");

        // Validate access for non-admin users
        if (!HasAdminBypass(auth))
        {
            var subordinateIds = await _dataSourceClient.GetSubordinateIdsAsync(auth.UserId);
            if (!subordinateIds.Contains(entry.UserId))
                return Result<MonthlyEntryResponse>.Failure(new ForbiddenError(
                    "access_denied", "Nie masz uprawnień do zatwierdzania wpisu tego pracownika"));
        }

        if (entry.Status != RcpTimeEntryStatus.Submitted)
            return ValidationFailure<MonthlyEntryResponse>("Można zatwierdzić tylko wpisy ze statusem 'Wysłane'");

        entry.Status = RcpTimeEntryStatus.Approved;
        entry.ApprovedAt = DateTime.UtcNow;
        entry.ApprovedByUserId = auth.UserId;
        entry.ApprovedByFullName = auth.FullName;
        entry.StatusChangedAt = DateTime.UtcNow;
        entry.StatusChangedByUserId = auth.UserId;
        entry.StatusChangedByFullName = auth.FullName;

        await _dataHandler.UpdateMonthlyEntryAsync(entry);

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            await AddCommentInternalAsync(entry.Id, auth, request.Comment, "Przełożony");
        }

        var year = entry.RcpPeriod?.Year ?? 0;
        var month = entry.RcpPeriod?.Month ?? 0;
        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(entry, year, month));
    }

    public async Task<Result<MonthlyEntryResponse>> RejectEntryAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request)
    {
        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(gid);
        if (entry == null)
            return NotFound<MonthlyEntryResponse>("Wpis nie został znaleziony");

        // Validate access for non-admin users
        if (!HasAdminBypass(auth))
        {
            var subordinateIds = await _dataSourceClient.GetSubordinateIdsAsync(auth.UserId);
            if (!subordinateIds.Contains(entry.UserId))
                return Result<MonthlyEntryResponse>.Failure(new ForbiddenError(
                    "access_denied", "Nie masz uprawnień do odrzucania wpisu tego pracownika"));
        }

        if (entry.Status != RcpTimeEntryStatus.Submitted)
            return ValidationFailure<MonthlyEntryResponse>("Można odrzucić tylko wpisy ze statusem 'Wysłane'");

        entry.Status = RcpTimeEntryStatus.Correction;
        entry.StatusChangedAt = DateTime.UtcNow;
        entry.StatusChangedByUserId = auth.UserId;
        entry.StatusChangedByFullName = auth.FullName;

        await _dataHandler.UpdateMonthlyEntryAsync(entry);

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            await AddCommentInternalAsync(entry.Id, auth, request.Comment, "Przełożony");
        }

        var year = entry.RcpPeriod?.Year ?? 0;
        var month = entry.RcpPeriod?.Month ?? 0;
        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(entry, year, month));
    }

    public async Task<Result<MonthlyEntryResponse>> ToSettlementAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request)
    {
        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(gid);
        if (entry == null)
            return NotFound<MonthlyEntryResponse>("Wpis nie został znaleziony");

        // Validate access for non-admin users
        if (!HasAdminBypass(auth))
        {
            var subordinateIds = await _dataSourceClient.GetSubordinateIdsAsync(auth.UserId);
            if (!subordinateIds.Contains(entry.UserId))
                return Result<MonthlyEntryResponse>.Failure(new ForbiddenError(
                    "access_denied", "Nie masz uprawnień do tego wpisu"));
        }

        if (entry.Status != RcpTimeEntryStatus.Approved)
            return ValidationFailure<MonthlyEntryResponse>("Można przekazać do rozliczenia tylko zatwierdzone wpisy");

        entry.Status = RcpTimeEntryStatus.Settlement;
        entry.StatusChangedAt = DateTime.UtcNow;
        entry.StatusChangedByUserId = auth.UserId;
        entry.StatusChangedByFullName = auth.FullName;

        await _dataHandler.UpdateMonthlyEntryAsync(entry);

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            await AddCommentInternalAsync(entry.Id, auth, request.Comment, "Przełożony");
        }

        var year = entry.RcpPeriod?.Year ?? 0;
        var month = entry.RcpPeriod?.Month ?? 0;
        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(entry, year, month));
    }

    #endregion

    #region Supervisor Monitoring

    /// <summary>
    /// Monitor subordinates' time entries - shows ALL statuses (Draft, Submitted, etc.)
    /// Supervisor can check progress before formal submission.
    /// Admin/Root sees ALL entries (to help users with issues).
    /// </summary>
    public async Task<Result<MonitorPeriodResponse>> GetSupervisorMonitorPeriodAsync(PortalAuthInfo auth, int year, int month)
    {
        var period = await _dataHandler.GetPeriodAsync(year, month);
        var allEntries = period != null
            ? await _dataHandler.GetMonthlyEntriesForPeriodAsync(period.Id)
            : [];

        var workingDays = CalculateWorkingDays(year, month);
        var summaries = new List<MonitorEntrySummary>();

        // Admin/Root sees ALL entries
        if (HasAdminBypass(auth))
        {
            foreach (var entry in allEntries)
            {
                var lastDay = entry.DayEntries
                    .Where(d => !d.IsDeleted)
                    .OrderByDescending(d => d.WorkDate)
                    .FirstOrDefault();

                summaries.Add(new MonitorEntrySummary(
                    entry.Gid,
                    entry.UserId,
                    entry.EmployeeFullName,
                    entry.Status,
                    Math.Round((decimal)entry.TotalMinutes / 60, 2),
                    entry.DayEntries.Count(d => !d.IsDeleted),
                    lastDay?.WorkDate.ToString("yyyy-MM-dd")
                ));
            }
        }
        else
        {
            // Supervisor sees only their subordinates
            var subordinateIds = await _dataSourceClient.GetSubordinateIdsAsync(auth.UserId);

            if (subordinateIds.Count == 0)
            {
                return Result<MonitorPeriodResponse>.Success(new MonitorPeriodResponse(
                    year, month, workingDays, []));
            }

            foreach (var userId in subordinateIds)
            {
                var entry = allEntries.FirstOrDefault(e => e.UserId == userId);

                if (entry != null)
                {
                    var lastDay = entry.DayEntries
                        .Where(d => !d.IsDeleted)
                        .OrderByDescending(d => d.WorkDate)
                        .FirstOrDefault();

                    summaries.Add(new MonitorEntrySummary(
                        entry.Gid,
                        userId,
                        entry.EmployeeFullName,
                        entry.Status,
                        Math.Round((decimal)entry.TotalMinutes / 60, 2),
                        entry.DayEntries.Count(d => !d.IsDeleted),
                        lastDay?.WorkDate.ToString("yyyy-MM-dd")
                    ));
                }
                else
                {
                    // No entry yet - show as Draft with zero progress
                    summaries.Add(new MonitorEntrySummary(
                        null,
                        userId,
                        null, // We don't have the name without an entry
                        RcpTimeEntryStatus.Draft,
                        0,
                        0,
                        null
                    ));
                }
            }
        }

        return Result<MonitorPeriodResponse>.Success(new MonitorPeriodResponse(
            year, month, workingDays, summaries.OrderBy(s => s.UserFullName ?? "").ToList()));
    }

    /// <summary>
    /// View subordinate's entry for monitoring (read-only, same as GetEntryByGidAsync but for monitoring).
    /// </summary>
    public async Task<Result<MonthlyEntryResponse>> GetSupervisorMonitorEntryAsync(PortalAuthInfo auth, string gid)
    {
        // Same as GetEntryByGidAsync - just reuse the logic
        return await GetEntryByGidAsync(auth, gid);
    }

    /// <summary>
    /// Calculate working days in a month (simplified - excludes weekends).
    /// </summary>
    private static int CalculateWorkingDays(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var workingDays = 0;

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
        }

        return workingDays;
    }

    #endregion

    #region HR/Payroll

    public async Task<Result<PeriodSummaryResponse>> GetPayrollPeriodEntriesAsync(PortalAuthInfo auth, int year, int month)
    {
        var period = await _dataHandler.GetPeriodAsync(year, month);
        if (period == null)
        {
            return Result<PeriodSummaryResponse>.Success(new PeriodSummaryResponse(year, month, []));
        }

        var entries = await _dataHandler.GetMonthlyEntriesForPeriodAsync(period.Id);

        // Filter only Settlement and Approved for HR
        var filtered = entries.Where(e =>
            e.Status == RcpTimeEntryStatus.Settlement ||
            e.Status == RcpTimeEntryStatus.Approved);

        var summaries = filtered.Select(e => new MonthlyEntrySummaryResponse(
            e.Gid,
            e.EmployeeFullName,
            e.UserGid,
            e.Status,
            e.TotalMinutes,
            FormatMinutes(e.TotalMinutes),
            e.SubmittedAt
        )).ToList();

        return Result<PeriodSummaryResponse>.Success(new PeriodSummaryResponse(year, month, summaries));
    }

    public async Task<Result<MonthlyEntryResponse>> ReturnForCorrectionAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request)
    {
        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(gid);
        if (entry == null)
            return NotFound<MonthlyEntryResponse>("Wpis nie został znaleziony");

        if (entry.Status != RcpTimeEntryStatus.Approved && entry.Status != RcpTimeEntryStatus.Settlement)
            return ValidationFailure<MonthlyEntryResponse>("Można zwrócić tylko zatwierdzone wpisy lub wpisy do rozliczenia");

        entry.Status = RcpTimeEntryStatus.Correction;
        entry.StatusChangedAt = DateTime.UtcNow;
        entry.StatusChangedByUserId = auth.UserId;
        entry.StatusChangedByFullName = auth.FullName;

        await _dataHandler.UpdateMonthlyEntryAsync(entry);

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            await AddCommentInternalAsync(entry.Id, auth, request.Comment, "Kadry");
        }

        var year = entry.RcpPeriod?.Year ?? 0;
        var month = entry.RcpPeriod?.Month ?? 0;
        return Result<MonthlyEntryResponse>.Success(MapToMonthlyResponse(entry, year, month));
    }

    #endregion

    #region Comments

    public async Task<Result<CommentResponse>> AddCommentAsync(PortalAuthInfo auth, string entryGid, CommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return ValidationFailure<CommentResponse>("Treść komentarza nie może być pusta");

        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(entryGid);
        if (entry == null)
            return NotFound<CommentResponse>("Wpis nie został znaleziony");

        var role = DetermineAuthorRole(auth, entry);
        var comment = await AddCommentInternalAsync(entry.Id, auth, request.Content, role);

        return Result<CommentResponse>.Success(MapToCommentResponse(comment));
    }

    public async Task<Result<List<CommentResponse>>> GetCommentsAsync(PortalAuthInfo auth, string entryGid)
    {
        var entry = await _dataHandler.GetMonthlyEntryByGidAsync(entryGid);
        if (entry == null)
            return NotFound<List<CommentResponse>>("Wpis nie został znaleziony");

        var comments = await _dataHandler.GetCommentsAsync(entry.Id);
        return Result<List<CommentResponse>>.Success(comments.Select(MapToCommentResponse).ToList());
    }

    private async Task<RcpEntryComment> AddCommentInternalAsync(long monthlyEntryId, PortalAuthInfo auth, string content, string role)
    {
        var comment = new RcpEntryComment
        {
            Gid = Guid.NewGuid().ToString(),
            MonthlyEntryId = monthlyEntryId,
            Content = content,
            AuthorUserId = auth.UserId,
            AuthorName = auth.FullName,
            AuthorRole = role
        };

        return await _dataHandler.CreateCommentAsync(comment);
    }

    private string DetermineAuthorRole(PortalAuthInfo auth, RcpMonthlyEntry entry)
    {
        if (entry.UserId == auth.UserId)
            return "Pracownik";

        if (auth.HasRole(AppRole.Admin) || auth.HasRole(AppRole.Ops))
            return "Admin";

        if (auth.HasRole(AppRole.Support))
            return "Support";

        return "Pracownik";
    }

    #endregion

    #region Mappers

    private static DayEntryResponse MapToDayResponse(RcpDayEntry entry)
    {
        var hours = entry.WorkMinutes / 60;
        var minutes = entry.WorkMinutes % 60;
        return new DayEntryResponse(
            entry.Gid,
            entry.WorkDate.ToString("yyyy-MM-dd"),
            $"{entry.StartTime.Hours:D2}:{entry.StartTime.Minutes:D2}",
            $"{entry.EndTime.Hours:D2}:{entry.EndTime.Minutes:D2}",
            entry.WorkMinutes,
            hours,
            minutes,
            $"{hours}:{minutes:D2}",
            Math.Round((decimal)entry.WorkMinutes / 60, 2),
            entry.Notes
        );
    }

    private static MonthlyEntryResponse MapToMonthlyResponse(RcpMonthlyEntry entry, int year, int month)
    {
        return new MonthlyEntryResponse(
            entry.Gid,
            year,
            month,
            entry.Status,
            entry.TotalMinutes,
            FormatMinutes(entry.TotalMinutes),
            Math.Round((decimal)entry.TotalMinutes / 60, 2),
            entry.EmployeeFullName,
            entry.UserGid,
            entry.SubmittedAt,
            entry.ApprovedAt,
            entry.ApprovedByFullName,
            entry.DayEntries.Select(MapToDayResponse).OrderBy(d => d.WorkDate).ToList(),
            entry.Comments.Select(MapToCommentResponse).OrderBy(c => c.CreatedAt).ToList()
        );
    }

    private static CommentResponse MapToCommentResponse(RcpEntryComment comment)
    {
        return new CommentResponse(
            comment.Gid,
            comment.Content,
            comment.AuthorName,
            comment.AuthorRole,
            comment.CreatedAt
        );
    }

    private static string FormatMinutes(int totalMinutes)
    {
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;
        return $"{hours}:{minutes:D2}";
    }

    #endregion
}
