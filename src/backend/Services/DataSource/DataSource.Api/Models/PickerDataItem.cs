namespace DataSource.Api.Models;

public class PickerDataItem
{
    public string Gid { get; set; } = string.Empty;
    public long RecordId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? SubLabel { get; set; }
}
