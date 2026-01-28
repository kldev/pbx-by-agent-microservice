using App.BaseData;

namespace CdrService.Data.Entities;

/// <summary>
/// Słownik przyczyn zakończenia połączenia (oparty o kody Q.850)
/// </summary>
public class TerminationCause : BaseTable
{
    /// <summary>
    /// Kod przyczyny (np. NORMAL_CLEARING, USER_BUSY)
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Kod Q.850 (jeśli dotyczy)
    /// </summary>
    public int? Q850Code { get; set; }

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
