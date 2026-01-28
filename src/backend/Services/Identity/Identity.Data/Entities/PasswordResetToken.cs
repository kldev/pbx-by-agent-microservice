namespace Identity.Data.Entities;

public class PasswordResetToken
{
    public long Id { get; set; }
    
    public long AppUserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }

    // Navigation
    public AppUser AppUser { get; set; } = null!;
}
