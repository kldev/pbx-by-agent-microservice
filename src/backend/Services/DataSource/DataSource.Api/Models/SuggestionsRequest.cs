using DataSource.Api.Enums;

namespace DataSource.Api.Models;

public class SuggestionsRequest
{
    public DataSourceType DataSourceType { get; set; }
    public int Limit { get; set; } = 25;
    public Dictionary<string, string>? ContextFilters { get; set; }
}
