namespace App.Shared.Web;

public interface ISeedService
{
    /// <summary>
    /// Seeduje tylko słowniki (jeśli puste)
    /// </summary>
    Task SeedDictionariesAsync();

    /// <summary>
    /// Seeduje dane showcase (prezentacyjne)
    /// </summary>
    Task SeedShowcaseDataAsync();

    /// <summary>
    /// Resetuje bazę i seeduje wszystko od nowa
    /// </summary>
    Task ResetAndSeedAsync(bool includeShowcase);
}