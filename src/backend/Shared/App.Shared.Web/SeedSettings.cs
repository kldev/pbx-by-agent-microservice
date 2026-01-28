namespace App.Shared.Web;

public class SeedSettings
{
    public const string SectionName = "Seed";

    /// <summary>
    /// Czy seed ma być uruchomiony automatycznie przy starcie aplikacji
    /// </summary>
    public bool RunOnStartup { get; set; } = false;

    /// <summary>
    /// Czy dodać dane prezentacyjne (showcase) - klienci, pracownicy, etc.
    /// </summary>
    public bool IncludeShowcaseData { get; set; } = false;

    /// <summary>
    /// Czy wyczyścić dane showcase przed ponownym seedowaniem
    /// </summary>
    public bool ResetShowcaseData { get; set; } = false;
}
