using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Microsoft.AspNetCore.Mvc;
using Rcp.Api.Features.TimeEntry.Model;

namespace Rcp.Api.Features.TimeEntry;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];
    private static readonly AppRole[] SupervisorRoles = [AppRole.Root, AppRole.Admin];
    private static readonly AppRole[] HrRoles = [AppRole.Root, AppRole.Admin, AppRole.Ops];

    public static void Map(WebApplication app)
    {
        var baseGroup = app.MapGroup("/api")
            .RequireAuthorization();

        // Own entries - dla pracownika
        MapOwnEntries(baseGroup.MapGroup("").WithTags("RCP-Employee"));

        // Supervisor - dla przełożonego
        MapSupervisorEndpoints(baseGroup.MapGroup("").WithTags("RCP-Supervisor"));

        // HR/Payroll - dla kadr
        MapPayrollEndpoints(baseGroup.MapGroup("").WithTags("RCP-Payroll"));

        // Comments - komentarze
        MapCommentEndpoints(baseGroup.MapGroup("").WithTags("RCP-Comments"));
    }

    private static void MapOwnEntries(RouteGroupBuilder group)
    {
        group.MapPost("/entry", SaveDayEntry)
            .WithName("SaveDayEntry")
            .WithSummary("Zapisz wpis dzienny")
            .Produces<DayEntryResponse>()
            .Produces<ApiErrorResponse>(400);

        group.MapDelete("/entry/{year:int}/{month:int}/{date}", DeleteDayEntry)
            .WithName("DeleteDayEntry")
            .WithSummary("Usuń wpis dzienny")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(400);

        group.MapPost("/my", GetMyMonthlyEntry)
            .WithName("GetMyMonthlyEntry")
            .WithSummary("Pobierz mój wpis miesięczny")
            .Produces<MonthlyEntryResponse>();

        group.MapPost("/my/submit", SubmitEntry)
            .WithName("SubmitEntry")
            .WithSummary("Wyślij do zatwierdzenia")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(400);
    }

    private static void MapSupervisorEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/supervisor/period", GetSupervisorPeriodEntries)
            .WithName("GetSupervisorPeriodEntries")
            .WithSummary("Lista wpisów do zatwierdzenia")
            .Produces<PeriodSummaryResponse>();

        group.MapPost("/supervisor/entry", GetEntryByGid)
            .WithName("GetEntryByGid")
            .WithSummary("Pobierz wpis po GID")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/supervisor/{gid}/approve", ApproveEntry)
            .WithName("ApproveEntry")
            .WithSummary("Zatwierdź wpis")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/supervisor/{gid}/reject", RejectEntry)
            .WithName("RejectEntry")
            .WithSummary("Odrzuć wpis")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/supervisor/{gid}/to-settlement", ToSettlement)
            .WithName("ToSettlement")
            .WithSummary("Przekaż do rozliczenia")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // Monitoring endpoints (read-only, all statuses)
        group.MapPost("/supervisor/monitor/period", GetSupervisorMonitorPeriod)
            .WithName("GetSupervisorMonitorPeriod")
            .WithSummary("Monitoruj postęp podwładnych")
            .WithDescription("Monitor subordinates' progress - shows all statuses")
            .Produces<MonitorPeriodResponse>();

        group.MapPost("/supervisor/monitor/entry", GetSupervisorMonitorEntry)
            .WithName("GetSupervisorMonitorEntry")
            .WithSummary("Podgląd wpisu podwładnego")
            .WithDescription("View subordinate's entry for monitoring")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(404);
    }

    private static void MapPayrollEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/payroll/period", GetPayrollPeriodEntries)
            .WithName("GetPayrollPeriodEntries")
            .WithSummary("Lista wpisów do rozliczenia")
            .Produces<PeriodSummaryResponse>();

        group.MapPost("/payroll/entry", GetPayrollEntry)
            .WithName("GetPayrollEntry")
            .WithSummary("Pobierz wpis do rozliczenia")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/payroll/return", ReturnForCorrection)
            .WithName("ReturnForCorrection")
            .WithSummary("Zwróć do poprawy")
            .Produces<MonthlyEntryResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/payroll/export", ExportToExcel)
            .WithName("ExportToExcel")
            .WithSummary("Eksportuj do Excel")
            .Produces<byte[]>(contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    private static void MapCommentEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/comments/list", GetComments)
            .WithName("GetComments")
            .WithSummary("Pobierz komentarze")
            .Produces<List<CommentResponse>>();

        group.MapPost("/{gid}/comments", AddComment)
            .WithName("AddComment")
            .WithSummary("Dodaj komentarz")
            .Produces<CommentResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);
    }

    #region Own Entries Handlers

    private static async Task<IResult> SaveDayEntry(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] SaveDayEntryRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.SaveDayEntryAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> DeleteDayEntry(
        ClaimsPrincipal user,
        IRcpService service,
        int year,
        int month,
        string date)
    {
        if (!DateTime.TryParse(date, out var workDate))
            return Results.BadRequest(new ApiErrorResponse { Code = "invalid_date", Message = "Nieprawidłowy format daty" });

        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteDayEntryAsync(auth, year, month, workDate),
            AllRoles);
    }

    private static async Task<IResult> GetMyMonthlyEntry(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetPeriodRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetMyMonthlyEntryAsync(auth, request.Year, request.Month),
            AllRoles);
    }

    private static async Task<IResult> SubmitEntry(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] SubmitRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.SubmitEntryAsync(auth, req),
            AllRoles);
    }

    #endregion

    #region Supervisor Handlers

    private static async Task<IResult> GetSupervisorPeriodEntries(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetPeriodRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetSupervisorPeriodEntriesAsync(auth, request.Year, request.Month),
            SupervisorRoles);
    }

    private static async Task<IResult> GetEntryByGid(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetByGidRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetEntryByGidAsync(auth, request.Gid),
            SupervisorRoles);
    }

    private static async Task<IResult> ApproveEntry(
        ClaimsPrincipal user,
        IRcpService service,
        string gid,
        [FromBody] ChangeStatusRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.ApproveEntryAsync(auth, gid, req),
            SupervisorRoles);
    }

    private static async Task<IResult> RejectEntry(
        ClaimsPrincipal user,
        IRcpService service,
        string gid,
        [FromBody] ChangeStatusRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.RejectEntryAsync(auth, gid, req),
            SupervisorRoles);
    }

    private static async Task<IResult> ToSettlement(
        ClaimsPrincipal user,
        IRcpService service,
        string gid,
        [FromBody] ChangeStatusRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.ToSettlementAsync(auth, gid, req),
            SupervisorRoles);
    }

    private static async Task<IResult> GetSupervisorMonitorPeriod(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetPeriodRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetSupervisorMonitorPeriodAsync(auth, request.Year, request.Month),
            SupervisorRoles);
    }

    private static async Task<IResult> GetSupervisorMonitorEntry(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetByGidRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetSupervisorMonitorEntryAsync(auth, request.Gid),
            SupervisorRoles);
    }

    #endregion

    #region Payroll Handlers

    private static async Task<IResult> GetPayrollPeriodEntries(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetPeriodRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetPayrollPeriodEntriesAsync(auth, request.Year, request.Month),
            HrRoles);
    }

    private static async Task<IResult> GetPayrollEntry(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetByGidRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetEntryByGidAsync(auth, request.Gid),
            HrRoles);
    }

    private static async Task<IResult> ReturnForCorrection(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] ReturnForCorrectionRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.ReturnForCorrectionAsync(auth, request.Gid, new ChangeStatusRequest(request.Comment)),
            HrRoles);
    }

    private static async Task<IResult> ExportToExcel(
        ClaimsPrincipal user,
        IRcpExcelExportService exportService,
        [FromBody] GetPeriodRequest request)
    {
        // Basic auth check
        var authInfo = App.Shared.Web.Security.ClaimsPrincipalExtensions.GetAuthInfo(user);
        if (authInfo == null)
            return Results.Unauthorized();

        var hasRole = HrRoles.Any(r => authInfo.HasRole(r));
        if (!hasRole)
            return Results.Forbid();

        var bytes = await exportService.GenerateMonthlyReportAsync(request.Year, request.Month);
        return Results.File(
            bytes,
            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileDownloadName: $"RCP_{request.Year}-{request.Month:D2}.xlsx"
        );
    }

    #endregion

    #region Comment Handlers

    private static async Task<IResult> GetComments(
        ClaimsPrincipal user,
        IRcpService service,
        [FromBody] GetByGidRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetCommentsAsync(auth, request.Gid),
            AllRoles);
    }

    private static async Task<IResult> AddComment(
        ClaimsPrincipal user,
        IRcpService service,
        string gid,
        [FromBody] CommentRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.AddCommentAsync(auth, gid, req),
            AllRoles);
    }

    #endregion
}

public record ReturnForCorrectionRequest(string Gid, string? Comment);
