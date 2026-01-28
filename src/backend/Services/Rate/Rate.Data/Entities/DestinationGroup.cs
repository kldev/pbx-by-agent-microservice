using App.BaseData;

namespace RateService.Data.Entities;

/// <summary>
/// Grupa destynacji (słownik) - np. Europe, Asia, Premium
/// </summary>
public class DestinationGroup : BaseDict
{
    public string Name { get; set; } = null!;
    public string NamePL { get; set; } = null!;
    public string? NameEN { get; set; }
    public string? Description { get; set; }

    // Navigation
    public ICollection<Rate> Rates { get; set; } = new List<Rate>();
}
