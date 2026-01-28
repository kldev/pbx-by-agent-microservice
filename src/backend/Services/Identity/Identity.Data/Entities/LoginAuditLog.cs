namespace Identity.Data.Entities;

public class LoginAuditLog
{
    public long Id { get; set; }
    public long AppUserId { get; set; }
    public DateTime LoginAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }

    public string UserEmail { get; set; } = null!;
    public string UserFullname { get; set; }  = null!;

    // Navigation
    public AppUser AppUser { get; set; } = null!;
}
