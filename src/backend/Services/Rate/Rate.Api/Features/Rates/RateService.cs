using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using RateService.Api.Definitions;
using RateService.Api.Features.Rates.Model;
using RateService.Api.Features.Tariffs;
using RateService.Data.Entities;

namespace RateService.Api.Features.Rates;

public class RateService : IRateService
{
    private readonly IRateDataHandler _dataHandler;
    private readonly ITariffDataHandler _tariffDataHandler;

    public RateService(IRateDataHandler dataHandler, ITariffDataHandler tariffDataHandler)
    {
        _dataHandler = dataHandler;
        _tariffDataHandler = tariffDataHandler;
    }

    public async Task<Result<PagedResult<RateResponse>>> GetListAsync(PortalAuthInfo auth, RateListFilter filter)
    {
        var (items, totalCount) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<RateResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<RateResponse>>.Success(result);
    }

    public async Task<Result<RateResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var rate = await _dataHandler.GetByGidAsync(gid);
        if (rate == null)
            return Result<RateResponse>.Failure(
                new NotFoundError(ErrorCodes.Rate.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.NotFound)));

        return Result<RateResponse>.Success(MapToResponse(rate));
    }

    public async Task<Result<RateResponse>> CreateAsync(PortalAuthInfo auth, CreateRateRequest request)
    {
        // Walidacja
        if (string.IsNullOrWhiteSpace(request.Prefix))
            return Result<RateResponse>.Failure(
                new ValidationError(ErrorCodes.Rate.PrefixRequired, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.PrefixRequired)));

        if (string.IsNullOrWhiteSpace(request.DestinationName))
            return Result<RateResponse>.Failure(
                new ValidationError(ErrorCodes.Rate.DestinationNameRequired, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.DestinationNameRequired)));

        if (request.RatePerMinute < 0)
            return Result<RateResponse>.Failure(
                new ValidationError(ErrorCodes.Rate.InvalidRatePerMinute, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.InvalidRatePerMinute)));

        // Sprawdź taryfę
        var tariff = await _tariffDataHandler.GetByGidAsync(request.TariffGid);
        if (tariff == null)
            return Result<RateResponse>.Failure(
                new ValidationError(ErrorCodes.Rate.TariffNotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.TariffNotFound)));

        // Sprawdź duplikat prefiksu w taryfie
        if (await _dataHandler.ExistsByPrefixInTariffAsync(tariff.Id, request.Prefix))
            return Result<RateResponse>.Failure(
                new BusinessLogicError(ErrorCodes.Rate.PrefixExists, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.PrefixExists)));

        // Utwórz
        var rate = new Rate
        {
            TariffId = tariff.Id,
            Prefix = request.Prefix,
            DestinationName = request.DestinationName,
            RatePerMinute = request.RatePerMinute,
            ConnectionFee = request.ConnectionFee,
            BillingIncrement = request.BillingIncrement,
            MinimumDuration = request.MinimumDuration,
            EffectiveFrom = request.EffectiveFrom ?? DateTime.UtcNow,
            EffectiveTo = request.EffectiveTo,
            DestinationGroupId = request.DestinationGroupId,
            Notes = request.Notes,
            CreatedByUserId = auth.UserId
        };

        await _dataHandler.CreateAsync(rate);

        // Reload z relacjami
        rate = await _dataHandler.GetByGidAsync(rate.Gid);
        return Result<RateResponse>.Success(MapToResponse(rate!));
    }

    public async Task<Result<RateResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateRateRequest request)
    {
        var rate = await _dataHandler.GetByGidAsync(gid);
        if (rate == null)
            return Result<RateResponse>.Failure(
                new NotFoundError(ErrorCodes.Rate.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.NotFound)));

        // Walidacja
        if (string.IsNullOrWhiteSpace(request.Prefix))
            return Result<RateResponse>.Failure(
                new ValidationError(ErrorCodes.Rate.PrefixRequired, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.PrefixRequired)));

        if (string.IsNullOrWhiteSpace(request.DestinationName))
            return Result<RateResponse>.Failure(
                new ValidationError(ErrorCodes.Rate.DestinationNameRequired, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.DestinationNameRequired)));

        // Sprawdź duplikat prefiksu (z wykluczeniem siebie)
        if (await _dataHandler.ExistsByPrefixInTariffAsync(rate.TariffId, request.Prefix, rate.Id))
            return Result<RateResponse>.Failure(
                new BusinessLogicError(ErrorCodes.Rate.PrefixExists, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.PrefixExists)));

        // Aktualizuj
        rate.Prefix = request.Prefix;
        rate.DestinationName = request.DestinationName;
        rate.RatePerMinute = request.RatePerMinute;
        rate.ConnectionFee = request.ConnectionFee;
        rate.BillingIncrement = request.BillingIncrement;
        rate.MinimumDuration = request.MinimumDuration;
        rate.EffectiveFrom = request.EffectiveFrom ?? rate.EffectiveFrom;
        rate.EffectiveTo = request.EffectiveTo;
        rate.IsActive = request.IsActive;
        rate.DestinationGroupId = request.DestinationGroupId;
        rate.Notes = request.Notes;
        rate.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(rate);
        return Result<RateResponse>.Success(MapToResponse(rate));
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var rate = await _dataHandler.GetByGidAsync(gid);
        if (rate == null)
            return Result<bool>.Failure(
                new NotFoundError(ErrorCodes.Rate.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.NotFound)));

        await _dataHandler.DeleteAsync(rate);
        return Result<bool>.Success(true);
    }

    public async Task<Result<RateResponse>> FindRateForNumberAsync(PortalAuthInfo auth, string tariffGid, string phoneNumber)
    {
        var tariff = await _tariffDataHandler.GetByGidAsync(tariffGid);
        if (tariff == null)
            return Result<RateResponse>.Failure(
                new NotFoundError(ErrorCodes.Tariff.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NotFound)));

        var rate = await _dataHandler.FindRateForNumberAsync(tariff.Id, phoneNumber);
        if (rate == null)
            return Result<RateResponse>.Failure(
                new NotFoundError(ErrorCodes.Rate.NoRateForNumber, ErrorCodeHelper.GetMessage(ErrorCodes.Rate.NoRateForNumber)));

        return Result<RateResponse>.Success(MapToResponse(rate));
    }

    private static RateResponse MapToResponse(Rate rate) => new()
    {
        Gid = rate.Gid,
        Prefix = rate.Prefix,
        DestinationName = rate.DestinationName,
        RatePerMinute = rate.RatePerMinute,
        ConnectionFee = rate.ConnectionFee,
        BillingIncrement = rate.BillingIncrement,
        MinimumDuration = rate.MinimumDuration,
        EffectiveFrom = rate.EffectiveFrom,
        EffectiveTo = rate.EffectiveTo,
        IsActive = rate.IsActive,
        Notes = rate.Notes,
        TariffGid = rate.Tariff?.Gid ?? "",
        TariffName = rate.Tariff?.Name ?? "",
        DestinationGroupName = rate.DestinationGroup?.Name,
        CreatedAt = rate.CreatedAt
    };
}
