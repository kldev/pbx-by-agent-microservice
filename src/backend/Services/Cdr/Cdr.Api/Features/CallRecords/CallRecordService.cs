using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Definitions;
using CdrService.Api.Features.CallRecords.Model;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallRecords;

public class CallRecordService : ICallRecordService
{
    private readonly ICallRecordDataHandler _dataHandler;

    public CallRecordService(ICallRecordDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<PagedResult<CallRecordResponse>>> GetListAsync(PortalAuthInfo auth, CallRecordListFilter filter)
    {
        var (items, totalCount) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<CallRecordResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<CallRecordResponse>>.Success(result);
    }

    public async Task<Result<CallRecordDetailResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var callRecord = await _dataHandler.GetByGidWithDetailsAsync(gid);
        if (callRecord == null)
            return Result<CallRecordDetailResponse>.Failure(
                new NotFoundError(ErrorCodes.CallRecord.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.NotFound)));

        return Result<CallRecordDetailResponse>.Success(MapToDetailResponse(callRecord));
    }

    public async Task<Result<CallRecordResponse>> CreateAsync(PortalAuthInfo auth, CreateCallRecordRequest request)
    {
        // Walidacja
        if (string.IsNullOrWhiteSpace(request.CallerId))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.CallerIdRequired, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.CallerIdRequired)));

        if (string.IsNullOrWhiteSpace(request.CalledNumber))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.CalledNumberRequired, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.CalledNumberRequired)));

        if (request.EndTime < request.StartTime)
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.EndTimeBeforeStartTime, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.EndTimeBeforeStartTime)));

        if (request.Duration < 0)
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidDuration, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidDuration)));

        if (request.TotalCost < 0)
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidCost, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidCost)));

        // Walidacja słowników
        if (!await _dataHandler.CallStatusExistsAsync(request.CallStatusId))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidCallStatus, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidCallStatus)));

        if (!await _dataHandler.TerminationCauseExistsAsync(request.TerminationCauseId))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidTerminationCause, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidTerminationCause)));

        if (!await _dataHandler.CallDirectionExistsAsync(request.CallDirectionId))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidCallDirection, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidCallDirection)));

        // Utwórz
        var callRecord = new CallRecord
        {
            CallUuid = request.CallUuid,
            CallerId = request.CallerId,
            CalledNumber = request.CalledNumber,
            StartTime = request.StartTime,
            AnswerTime = request.AnswerTime,
            EndTime = request.EndTime,
            Duration = request.Duration,
            BillableSeconds = request.BillableSeconds,
            CallStatusId = request.CallStatusId,
            TerminationCauseId = request.TerminationCauseId,
            CallDirectionId = request.CallDirectionId,
            SourceGatewayGid = request.SourceGatewayGid,
            SourceGatewayName = request.SourceGatewayName,
            DestinationGatewayGid = request.DestinationGatewayGid,
            DestinationGatewayName = request.DestinationGatewayName,
            TariffGid = request.TariffGid,
            TariffName = request.TariffName,
            RatePerMinute = request.RatePerMinute,
            ConnectionFee = request.ConnectionFee,
            BillingIncrement = request.BillingIncrement,
            CurrencyCode = request.CurrencyCode,
            DestinationName = request.DestinationName,
            MatchedPrefix = request.MatchedPrefix,
            TotalCost = request.TotalCost,
            CustomerGid = request.CustomerGid,
            CustomerName = request.CustomerName,
            SipAccountGid = request.SipAccountGid,
            SipAccountUsername = request.SipAccountUsername,
            UserData = request.UserData,
            RawCdrJson = request.RawCdrJson,
            CreatedByUserId = auth.UserId
        };

        await _dataHandler.CreateAsync(callRecord);

        // Pobierz z relacjami do mapowania
        var created = await _dataHandler.GetByGidWithDetailsAsync(callRecord.Gid);
        return Result<CallRecordResponse>.Success(MapToResponse(created!));
    }

    public async Task<Result<CallRecordResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateCallRecordRequest request)
    {
        var callRecord = await _dataHandler.GetByGidAsync(gid);
        if (callRecord == null)
            return Result<CallRecordResponse>.Failure(
                new NotFoundError(ErrorCodes.CallRecord.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.NotFound)));

        // Walidacja
        if (request.EndTime < callRecord.StartTime)
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.EndTimeBeforeStartTime, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.EndTimeBeforeStartTime)));

        if (request.Duration < 0)
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidDuration, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidDuration)));

        if (request.TotalCost < 0)
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidCost, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidCost)));

        if (!await _dataHandler.CallStatusExistsAsync(request.CallStatusId))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidCallStatus, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidCallStatus)));

        if (!await _dataHandler.TerminationCauseExistsAsync(request.TerminationCauseId))
            return Result<CallRecordResponse>.Failure(
                new ValidationError(ErrorCodes.CallRecord.InvalidTerminationCause, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.InvalidTerminationCause)));

        // Aktualizuj
        callRecord.AnswerTime = request.AnswerTime;
        callRecord.EndTime = request.EndTime;
        callRecord.Duration = request.Duration;
        callRecord.BillableSeconds = request.BillableSeconds;
        callRecord.CallStatusId = request.CallStatusId;
        callRecord.TerminationCauseId = request.TerminationCauseId;
        callRecord.TotalCost = request.TotalCost;
        callRecord.UserData = request.UserData;
        callRecord.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(callRecord);

        var updated = await _dataHandler.GetByGidWithDetailsAsync(callRecord.Gid);
        return Result<CallRecordResponse>.Success(MapToResponse(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var callRecord = await _dataHandler.GetByGidAsync(gid);
        if (callRecord == null)
            return Result<bool>.Failure(
                new NotFoundError(ErrorCodes.CallRecord.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.CallRecord.NotFound)));

        await _dataHandler.DeleteAsync(callRecord);
        return Result<bool>.Success(true);
    }

    private static CallRecordResponse MapToResponse(CallRecord c) => new()
    {
        Gid = c.Gid,
        CallerId = c.CallerId,
        CalledNumber = c.CalledNumber,
        StartTime = c.StartTime,
        EndTime = c.EndTime,
        Duration = c.Duration,
        BillableSeconds = c.BillableSeconds,
        CallStatusCode = c.CallStatus?.Code ?? "",
        CallStatusName = c.CallStatus?.NamePL ?? "",
        CallDirectionCode = c.CallDirection?.Code ?? "",
        TotalCost = c.TotalCost,
        CurrencyCode = c.CurrencyCode,
        CustomerName = c.CustomerName,
        CreatedAt = c.CreatedAt
    };

    private static CallRecordDetailResponse MapToDetailResponse(CallRecord c) => new()
    {
        Gid = c.Gid,
        CallerId = c.CallerId,
        CalledNumber = c.CalledNumber,
        StartTime = c.StartTime,
        EndTime = c.EndTime,
        Duration = c.Duration,
        BillableSeconds = c.BillableSeconds,
        CallStatusCode = c.CallStatus?.Code ?? "",
        CallStatusName = c.CallStatus?.NamePL ?? "",
        CallDirectionCode = c.CallDirection?.Code ?? "",
        TotalCost = c.TotalCost,
        CurrencyCode = c.CurrencyCode,
        CustomerName = c.CustomerName,
        CreatedAt = c.CreatedAt,
        CallUuid = c.CallUuid,
        AnswerTime = c.AnswerTime,
        TerminationCauseCode = c.TerminationCause?.Code,
        TerminationCauseName = c.TerminationCause?.NamePL,
        SourceGatewayGid = c.SourceGatewayGid,
        SourceGatewayName = c.SourceGatewayName,
        DestinationGatewayGid = c.DestinationGatewayGid,
        DestinationGatewayName = c.DestinationGatewayName,
        TariffGid = c.TariffGid,
        TariffName = c.TariffName,
        RatePerMinute = c.RatePerMinute,
        ConnectionFee = c.ConnectionFee,
        BillingIncrement = c.BillingIncrement,
        DestinationName = c.DestinationName,
        MatchedPrefix = c.MatchedPrefix,
        CustomerGid = c.CustomerGid,
        SipAccountGid = c.SipAccountGid,
        SipAccountUsername = c.SipAccountUsername,
        UserData = c.UserData
    };
}
