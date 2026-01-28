namespace App.BaseData;

public abstract class BaseAuditableTable : BaseTable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? CreatedByUserId { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}