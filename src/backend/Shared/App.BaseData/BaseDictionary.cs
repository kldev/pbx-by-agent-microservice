namespace App.BaseData;

/// <summary>
/// Bazowa klasa dla wszystkich słowników
/// </summary>
public abstract class BaseDict
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Słownik z podstawowymi tłumaczeniami (PL, EN)
/// </summary>
public abstract class BaseTranslatedDict : BaseDict
{
    public string NamePL { get; set; } = null!;
    public string? NameEN { get; set; }
}

/// <summary>
/// Słownik z kategorią i rozszerzonymi tłumaczeniami
/// </summary>
public abstract class BaseCategorizedDict : BaseTranslatedDict
{
    public string? Category { get; set; }
}

/// <summary>
/// Słownik z wielojęzycznymi nazwami (PL, EN)
/// </summary>
public abstract class BaseMultiLangDict : BaseTranslatedDict
{
}
