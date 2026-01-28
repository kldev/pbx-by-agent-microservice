using App.BaseData;

namespace CdrService.Data.Entities;

/// <summary>
/// Słownik statusów połączeń
/// </summary>
public class CallStatus : BaseTable
{
    /// <summary>
    /// Kod statusu (np. COMPLETED, FAILED)
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Nazwa po polsku
    /// </summary>
    public string NamePL { get; set; } = null!;

    /// <summary>
    /// Nazwa po angielsku
    /// </summary>
    public string NameEN { get; set; } = null!;

    /// <summary>
    /// Opis
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kolejność sortowania
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Czy aktywny
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<CallRecord> CallRecords { get; set; } = new List<CallRecord>();
}
