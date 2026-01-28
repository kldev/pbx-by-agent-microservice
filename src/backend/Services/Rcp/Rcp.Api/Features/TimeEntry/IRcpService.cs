using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Rcp.Api.Features.TimeEntry.Model;

namespace Rcp.Api.Features.TimeEntry;

public interface IRcpService
{
    // Own entries (ResponsiblePerson)
    Task<Result<DayEntryResponse>> SaveDayEntryAsync(PortalAuthInfo auth, SaveDayEntryRequest request);
    Task<Result<bool>> DeleteDayEntryAsync(PortalAuthInfo auth, int year, int month, DateTime workDate);
    Task<Result<MonthlyEntryResponse>> GetMyMonthlyEntryAsync(PortalAuthInfo auth, int year, int month);
    Task<Result<MonthlyEntryResponse>> SubmitEntryAsync(PortalAuthInfo auth, SubmitRequest request);

    // Supervisor - approval
    Task<Result<PeriodSummaryResponse>> GetSupervisorPeriodEntriesAsync(PortalAuthInfo auth, int year, int month);
    Task<Result<MonthlyEntryResponse>> GetEntryByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<MonthlyEntryResponse>> ApproveEntryAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request);
    Task<Result<MonthlyEntryResponse>> RejectEntryAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request);
    Task<Result<MonthlyEntryResponse>> ToSettlementAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request);

    // Supervisor - monitoring (read-only, all statuses)
    Task<Result<MonitorPeriodResponse>> GetSupervisorMonitorPeriodAsync(PortalAuthInfo auth, int year, int month);
    Task<Result<MonthlyEntryResponse>> GetSupervisorMonitorEntryAsync(PortalAuthInfo auth, string gid);

    // HR/Payroll
    Task<Result<PeriodSummaryResponse>> GetPayrollPeriodEntriesAsync(PortalAuthInfo auth, int year, int month);
    Task<Result<MonthlyEntryResponse>> ReturnForCorrectionAsync(PortalAuthInfo auth, string gid, ChangeStatusRequest request);

    // Comments
    Task<Result<CommentResponse>> AddCommentAsync(PortalAuthInfo auth, string entryGid, CommentRequest request);
    Task<Result<List<CommentResponse>>> GetCommentsAsync(PortalAuthInfo auth, string entryGid);
}
