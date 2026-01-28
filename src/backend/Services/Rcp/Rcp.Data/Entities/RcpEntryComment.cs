using App.BaseData;

namespace Rcp.Data.Entities;

/// <summary>
/// Komentarz do wpisu RCP
/// </summary>
public class RcpEntryComment : BaseAuditableTable
{
    public long MonthlyEntryId { get; set; }
    public string Content { get; set; } = null!;

    /// <summary>ID użytkownika (z Identity service)</summary>
    public long AuthorUserId { get; set; }

    /// <summary>Snapshot imienia i nazwiska autora</summary>
    public string? AuthorName { get; set; }

    /// <summary>Rola autora (Pracownik/Przełożony/Kadry)</summary>
    public string? AuthorRole { get; set; }

    // Navigation
    public virtual RcpMonthlyEntry? MonthlyEntry { get; set; }
}
