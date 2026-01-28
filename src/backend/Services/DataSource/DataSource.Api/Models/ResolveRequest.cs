using DataSource.Api.Enums;

namespace DataSource.Api.Models;

public class ResolveRequest
{
    public DataSourceType DataSourceType { get; set; }
    public List<string>? Gids { get; set; }
    public List<long>? Ids { get; set; }
}
