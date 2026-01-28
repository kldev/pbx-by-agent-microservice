using DataSource.Api.Enums;

namespace DataSource.Api.Models;

public class SearchRequest
{
    public DataSourceType DataSourceType { get; set; }
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 25;
    public Dictionary<string, string>? Filters { get; set; }
    public Dictionary<string, string>? ContextFilters { get; set; }
}
