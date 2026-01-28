namespace App.BaseData;

public abstract class BaseTable
{
    public long Id { get; set; }
    public string Gid { get; set; } = Guid.NewGuid().ToString("N");
}
