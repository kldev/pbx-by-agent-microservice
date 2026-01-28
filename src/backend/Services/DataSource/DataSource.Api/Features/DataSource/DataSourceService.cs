using Common.Toolkit.ResultPattern;
using DataSource.Api.Enums;
using DataSource.Api.Models;
using DataSource.Data;
using Microsoft.EntityFrameworkCore;

namespace DataSource.Api.Features.DataSource;

public class DataSourceService : IDataSourceService
{
    private readonly DataSourceDbContext _context;

    public DataSourceService(DataSourceDbContext context)
    {
        _context = context;
    }

    #region Suggestions

    public async Task<Result<PickerDataResponse>> GetSuggestionsAsync(SuggestionsRequest request)
    {
        var limit = Math.Min(request.Limit, 50);

        var items = request.DataSourceType switch
        {
            DataSourceType.Clients => await GetClientsSuggestionsAsync(limit),
            DataSourceType.Countries => await GetCountriesSuggestionsAsync(limit),
            DataSourceType.UsersSales => await GetUsersSalesSuggestionsAsync(limit),
            DataSourceType.UsersAll => await GetUsersAllSuggestionsAsync(limit),
            DataSourceType.Provinces => await GetProvincesSuggestionsAsync(limit, request.ContextFilters),
            DataSourceType.Sbu => await GetSbuSuggestionsAsync(limit),
            DataSourceType.Teams => await GetTeamsSuggestionsAsync(limit, request.ContextFilters),
            DataSourceType.Benefits => await GetBenefitsSuggestionsAsync(limit),
            DataSourceType.Certificates => await GetCertificatesSuggestionsAsync(limit),
            DataSourceType.Tools => await GetToolsSuggestionsAsync(limit),
            DataSourceType.Traits => await GetTraitsSuggestionsAsync(limit),
            DataSourceType.Occupations => await GetOccupationsSuggestionsAsync(limit),
            DataSourceType.Positions => await GetPositionsSuggestionsAsync(limit),
            _ => new List<PickerDataItem>()
        };

        return Result<PickerDataResponse>.Success(new PickerDataResponse
        {
            Items = items,
            TotalCount = items.Count
        });
    }

    private async Task<List<PickerDataItem>> GetClientsSuggestionsAsync(int limit)
    {
        return await _context.Clients
            .OrderByDescending(x => x.ModifiedAt ?? x.CreatedAt)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetCountriesSuggestionsAsync(int limit)
    {
        return await _context.Countries
            .OrderByDescending(x => x.IsFavorite)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetUsersSalesSuggestionsAsync(int limit)
    {
        return await _context.UsersSales
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetProvincesSuggestionsAsync(int limit, Dictionary<string, string>? contextFilters)
    {
        var query = _context.Provinces.AsQueryable();

        // Optional: filter by country if provided in context
        // if (contextFilters?.TryGetValue("CountryGid", out var countryGid) == true)
        // {
        //     query = query.Where(x => x.CountryGid == countryGid);
        // }

        return await query
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetUsersAllSuggestionsAsync(int limit)
    {
        return await _context.UsersAll
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetSbuSuggestionsAsync(int limit)
    {
        return await _context.Sbu
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetTeamsSuggestionsAsync(int limit, Dictionary<string, string>? contextFilters)
    {
        var query = _context.Teams.AsQueryable();

        // Support both SbuId (numeric) and SbuGid (code) filters
        if (contextFilters?.TryGetValue("SbuId", out var sbuIdStr) == true
            && !string.IsNullOrEmpty(sbuIdStr)
            && int.TryParse(sbuIdStr, out var sbuId))
        {
            query = query.Where(x => x.SbuId == sbuId);
        }
        else if (contextFilters?.TryGetValue("SbuGid", out var sbuGid) == true && !string.IsNullOrEmpty(sbuGid))
        {
            query = query.Where(x => x.SbuGid == sbuGid);
        }

        return await query
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetBenefitsSuggestionsAsync(int limit)
    {
        return await _context.Benefits
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetCertificatesSuggestionsAsync(int limit)
    {
        return await _context.Certificates
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetToolsSuggestionsAsync(int limit)
    {
        return await _context.Tools
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetTraitsSuggestionsAsync(int limit)
    {
        return await _context.Traits
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetOccupationsSuggestionsAsync(int limit)
    {
        return await _context.OccupationCodes
            .Where(x => x.IsSelectable)
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> GetPositionsSuggestionsAsync(int limit)
    {
        return await _context.Positions
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    #endregion

    #region Search

    public async Task<Result<PickerDataResponse>> SearchAsync(SearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length < 2)
            return Result<PickerDataResponse>.Failure(new ValidationError("QUERY_TOO_SHORT", "Query must be at least 2 characters"));

        var limit = Math.Min(request.Limit, 100);
        var query = request.Query.Trim();

        var items = request.DataSourceType switch
        {
            DataSourceType.Clients => await SearchClientsAsync(query, limit),
            DataSourceType.Countries => await SearchCountriesAsync(query, limit),
            DataSourceType.UsersSales => await SearchUsersSalesAsync(query, limit),
            DataSourceType.UsersAll => await SearchUsersAllAsync(query, limit),
            DataSourceType.Provinces => await SearchProvincesAsync(query, limit),
            DataSourceType.Sbu => await SearchSbuAsync(query, limit),
            DataSourceType.Teams => await SearchTeamsAsync(query, limit, request.ContextFilters),
            DataSourceType.Benefits => await SearchBenefitsAsync(query, limit),
            DataSourceType.Certificates => await SearchCertificatesAsync(query, limit),
            DataSourceType.Tools => await SearchToolsAsync(query, limit),
            DataSourceType.Traits => await SearchTraitsAsync(query, limit),
            DataSourceType.Occupations => await SearchOccupationsAsync(query, limit),
            DataSourceType.Positions => await SearchPositionsAsync(query, limit),
            _ => new List<PickerDataItem>()
        };

        return Result<PickerDataResponse>.Success(new PickerDataResponse
        {
            Items = items,
            TotalCount = items.Count
        });
    }

    private async Task<List<PickerDataItem>> SearchClientsAsync(string query, int limit)
    {
        return await _context.Clients
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchCountriesAsync(string query, int limit)
    {
        return await _context.Countries
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderByDescending(x => x.IsFavorite)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchUsersSalesAsync(string query, int limit)
    {
        return await _context.UsersSales
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchProvincesAsync(string query, int limit)
    {
        return await _context.Provinces
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchUsersAllAsync(string query, int limit)
    {
        return await _context.UsersAll
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchSbuAsync(string query, int limit)
    {
        return await _context.Sbu
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchTeamsAsync(string query, int limit, Dictionary<string, string>? contextFilters)
    {
        var dbQuery = _context.Teams.AsQueryable();

        // Support both SbuId (numeric) and SbuGid (code) filters
        if (contextFilters?.TryGetValue("SbuId", out var sbuIdStr) == true
            && !string.IsNullOrEmpty(sbuIdStr)
            && int.TryParse(sbuIdStr, out var sbuId))
        {
            dbQuery = dbQuery.Where(x => x.SbuId == sbuId);
        }
        else if (contextFilters?.TryGetValue("SbuGid", out var sbuGid) == true && !string.IsNullOrEmpty(sbuGid))
        {
            dbQuery = dbQuery.Where(x => x.SbuGid == sbuGid);
        }

        return await dbQuery
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchBenefitsAsync(string query, int limit)
    {
        return await _context.Benefits
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchCertificatesAsync(string query, int limit)
    {
        return await _context.Certificates
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchToolsAsync(string query, int limit)
    {
        return await _context.Tools
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchTraitsAsync(string query, int limit)
    {
        return await _context.Traits
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchOccupationsAsync(string query, int limit)
    {
        return await _context.OccupationCodes
            .Where(x => x.IsSelectable && (x.Label.Contains(query) || x.Gid.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> SearchPositionsAsync(string query, int limit)
    {
        return await _context.Positions
            .Where(x => x.Label.Contains(query) || (x.SubLabel != null && x.SubLabel.Contains(query)))
            .OrderBy(x => x.Label)
            .Take(limit)
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    #endregion

    #region Resolve

    public async Task<Result<PickerDataResponse>> ResolveAsync(ResolveRequest request)
    {
        if ((request.Gids == null || !request.Gids.Any()) &&
            (request.Ids == null || !request.Ids.Any()))
            return Result<PickerDataResponse>.Failure(new ValidationError("MISSING_IDS", "Either Gids or Ids must be provided"));

        var items = request.DataSourceType switch
        {
            DataSourceType.Clients => await ResolveClientsAsync(request.Gids, request.Ids),
            DataSourceType.Countries => await ResolveCountriesAsync(request.Gids, request.Ids),
            DataSourceType.UsersSales => await ResolveUsersSalesAsync(request.Gids, request.Ids),
            DataSourceType.UsersAll => await ResolveUsersAllAsync(request.Gids, request.Ids),
            DataSourceType.Provinces => await ResolveProvincesAsync(request.Gids, request.Ids),
            DataSourceType.Sbu => await ResolveSbuAsync(request.Gids, request.Ids),
            DataSourceType.Teams => await ResolveTeamsAsync(request.Gids, request.Ids),
            DataSourceType.Benefits => await ResolveBenefitsAsync(request.Gids, request.Ids),
            DataSourceType.Certificates => await ResolveCertificatesAsync(request.Gids, request.Ids),
            DataSourceType.Tools => await ResolveToolsAsync(request.Gids, request.Ids),
            DataSourceType.Traits => await ResolveTraitsAsync(request.Gids, request.Ids),
            DataSourceType.Occupations => await ResolveOccupationsAsync(request.Gids, request.Ids),
            DataSourceType.Positions => await ResolvePositionsAsync(request.Gids, request.Ids),
            _ => new List<PickerDataItem>()
        };

        return Result<PickerDataResponse>.Success(new PickerDataResponse
        {
            Items = items,
            TotalCount = items.Count
        });
    }

    private async Task<List<PickerDataItem>> ResolveClientsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Clients.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveCountriesAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Countries.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveUsersSalesAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.UsersSales.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveProvincesAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Provinces.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveUsersAllAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.UsersAll.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveSbuAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Sbu.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveTeamsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Teams.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveBenefitsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Benefits.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveCertificatesAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Certificates.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveToolsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Tools.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveTraitsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Traits.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolveOccupationsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.OccupationCodes.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    private async Task<List<PickerDataItem>> ResolvePositionsAsync(List<string>? gids, List<long>? ids)
    {
        var query = _context.Positions.AsQueryable();

        if (gids?.Any() == true)
            query = query.Where(x => gids.Contains(x.Gid));
        else if (ids?.Any() == true)
            query = query.Where(x => ids.Contains(x.RecordId));

        return await query
            .Select(x => new PickerDataItem
            {
                Gid = x.Gid,
                RecordId = x.RecordId,
                Label = x.Label,
                SubLabel = x.SubLabel
            })
            .ToListAsync();
    }

    #endregion

    #region Types

    public List<DataSourceTypeInfo> GetDataSourceTypes()
    {
        return Enum.GetValues<DataSourceType>()
            .Select(t => new DataSourceTypeInfo
            {
                Value = (int)t,
                Name = t.ToString()
            })
            .ToList();
    }

    #endregion

    #region Subordinates

    /// <summary>
    /// Get subordinates for a given supervisor.
    /// Used by RCP for filtering supervisor views.
    /// </summary>
    public async Task<Result<PickerDataResponse>> GetSubordinatesAsync(SubordinatesRequest request)
    {
        // Use AsNoTracking for read-only query
        var items = await _context.UserSubordinatesQuery
            .Where(u => u.SupervisorId == request.SupervisorId)
            .OrderBy(u => u.Label)
            .Select(u => new PickerDataItem
            {
                RecordId = u.RecordId,
                Gid = u.Gid,
                Label = u.Label,
                SubLabel = u.SubLabel
            })
            .ToListAsync();

        return Result<PickerDataResponse>.Success(new PickerDataResponse
        {
            Items = items,
            TotalCount = items.Count
        });
    }

    #endregion
}
