namespace DataSource.Data.Entities;

public abstract class ViewEntityBase
{
    public long RecordId { get; set; }
    public string Gid { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? SubLabel { get; set; }
}
