namespace DataSource.Api.Models;

public class PickerDataResponse
{
    public List<PickerDataItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
}
