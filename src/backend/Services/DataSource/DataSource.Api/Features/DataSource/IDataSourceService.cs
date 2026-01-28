using Common.Toolkit.ResultPattern;
using DataSource.Api.Models;

namespace DataSource.Api.Features.DataSource;

public interface IDataSourceService
{
    Task<Result<PickerDataResponse>> GetSuggestionsAsync(SuggestionsRequest request);
    Task<Result<PickerDataResponse>> SearchAsync(SearchRequest request);
    Task<Result<PickerDataResponse>> ResolveAsync(ResolveRequest request);
    List<DataSourceTypeInfo> GetDataSourceTypes();

    /// <summary>
    /// Get subordinates for a given supervisor (used by RCP for filtering).
    /// </summary>
    Task<Result<PickerDataResponse>> GetSubordinatesAsync(SubordinatesRequest request);
}
