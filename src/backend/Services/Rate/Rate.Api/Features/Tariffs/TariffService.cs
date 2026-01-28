using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using RateService.Api.Definitions;
using RateService.Api.Features.Tariffs.Model;
using RateService.Data.Entities;

namespace RateService.Api.Features.Tariffs;

public class TariffService : ITariffService
{
    private readonly ITariffDataHandler _dataHandler;

    public TariffService(ITariffDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<PagedResult<TariffResponse>>> GetListAsync(PortalAuthInfo auth, TariffListFilter filter)
    {
        var (items, totalCount) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<TariffResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<TariffResponse>>.Success(result);
    }

    public async Task<Result<TariffDetailResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var tariff = await _dataHandler.GetByGidWithRatesAsync(gid);
        if (tariff == null)
            return Result<TariffDetailResponse>.Failure(
                new NotFoundError(ErrorCodes.Tariff.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NotFound)));

        return Result<TariffDetailResponse>.Success(MapToDetailResponse(tariff));
    }

    public async Task<Result<TariffResponse>> CreateAsync(PortalAuthInfo auth, CreateTariffRequest request)
    {
        // Walidacja
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<TariffResponse>.Failure(
                new ValidationError(ErrorCodes.Tariff.NameRequired, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NameRequired)));

        if (request.BillingIncrement <= 0)
            return Result<TariffResponse>.Failure(
                new ValidationError(ErrorCodes.Tariff.InvalidBillingIncrement, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.InvalidBillingIncrement)));

        // Sprawdź duplikat
        if (await _dataHandler.ExistsByNameAsync(request.Name))
            return Result<TariffResponse>.Failure(
                new BusinessLogicError(ErrorCodes.Tariff.NameExists, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NameExists)));

        // Jeśli ustawiamy jako domyślną, wyczyść flagę u innych
        if (request.IsDefault)
        {
            await _dataHandler.ClearDefaultFlagAsync();
        }

        // Utwórz
        var tariff = new Tariff
        {
            Name = request.Name,
            Description = request.Description,
            CurrencyCode = request.CurrencyCode,
            IsDefault = request.IsDefault,
            ValidFrom = request.ValidFrom ?? DateTime.UtcNow,
            ValidTo = request.ValidTo,
            BillingIncrement = request.BillingIncrement,
            MinimumDuration = request.MinimumDuration,
            ConnectionFee = request.ConnectionFee,
            CreatedByUserId = auth.UserId
        };

        await _dataHandler.CreateAsync(tariff);
        return Result<TariffResponse>.Success(MapToResponse(tariff));
    }

    public async Task<Result<TariffResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateTariffRequest request)
    {
        var tariff = await _dataHandler.GetByGidAsync(gid);
        if (tariff == null)
            return Result<TariffResponse>.Failure(
                new NotFoundError(ErrorCodes.Tariff.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NotFound)));

        // Walidacja
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<TariffResponse>.Failure(
                new ValidationError(ErrorCodes.Tariff.NameRequired, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NameRequired)));

        // Sprawdź duplikat (z wykluczeniem siebie)
        if (await _dataHandler.ExistsByNameAsync(request.Name, tariff.Id))
            return Result<TariffResponse>.Failure(
                new BusinessLogicError(ErrorCodes.Tariff.NameExists, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NameExists)));

        // Jeśli ustawiamy jako domyślną, wyczyść flagę u innych
        if (request.IsDefault && !tariff.IsDefault)
        {
            await _dataHandler.ClearDefaultFlagAsync(tariff.Id);
        }

        // Aktualizuj
        tariff.Name = request.Name;
        tariff.Description = request.Description;
        tariff.CurrencyCode = request.CurrencyCode;
        tariff.IsDefault = request.IsDefault;
        tariff.IsActive = request.IsActive;
        tariff.ValidFrom = request.ValidFrom ?? tariff.ValidFrom;
        tariff.ValidTo = request.ValidTo;
        tariff.BillingIncrement = request.BillingIncrement;
        tariff.MinimumDuration = request.MinimumDuration;
        tariff.ConnectionFee = request.ConnectionFee;
        tariff.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(tariff);
        return Result<TariffResponse>.Success(MapToResponse(tariff));
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var tariff = await _dataHandler.GetByGidAsync(gid);
        if (tariff == null)
            return Result<bool>.Failure(
                new NotFoundError(ErrorCodes.Tariff.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NotFound)));

        if (tariff.IsDefault)
            return Result<bool>.Failure(
                new BusinessLogicError(ErrorCodes.Tariff.CannotDeleteDefault, ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.CannotDeleteDefault)));

        await _dataHandler.DeleteAsync(tariff);
        return Result<bool>.Success(true);
    }

    private static TariffResponse MapToResponse(Tariff tariff) => new()
    {
        Gid = tariff.Gid,
        Name = tariff.Name,
        Description = tariff.Description,
        CurrencyCode = tariff.CurrencyCode,
        IsDefault = tariff.IsDefault,
        IsActive = tariff.IsActive,
        ValidFrom = tariff.ValidFrom,
        ValidTo = tariff.ValidTo,
        BillingIncrement = tariff.BillingIncrement,
        MinimumDuration = tariff.MinimumDuration,
        ConnectionFee = tariff.ConnectionFee,
        RatesCount = tariff.Rates?.Count ?? 0,
        CreatedAt = tariff.CreatedAt
    };

    private static TariffDetailResponse MapToDetailResponse(Tariff tariff) => new()
    {
        Gid = tariff.Gid,
        Name = tariff.Name,
        Description = tariff.Description,
        CurrencyCode = tariff.CurrencyCode,
        IsDefault = tariff.IsDefault,
        IsActive = tariff.IsActive,
        ValidFrom = tariff.ValidFrom,
        ValidTo = tariff.ValidTo,
        BillingIncrement = tariff.BillingIncrement,
        MinimumDuration = tariff.MinimumDuration,
        ConnectionFee = tariff.ConnectionFee,
        RatesCount = tariff.Rates?.Count ?? 0,
        CreatedAt = tariff.CreatedAt,
        Rates = tariff.Rates?.Select(r => new TariffRateResponse
        {
            Gid = r.Gid,
            Prefix = r.Prefix,
            DestinationName = r.DestinationName,
            RatePerMinute = r.RatePerMinute,
            IsActive = r.IsActive
        }) ?? Enumerable.Empty<TariffRateResponse>()
    };
}
