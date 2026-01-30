using System.Globalization;
using System.Text;
using Identity.Api.Infrastructure;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// UserData for seeding test users.
/// </summary>
public static partial class UserData
{
    // Default password for all test users: "Agent666"
    internal static readonly string DefaultPasswordHash = PasswordHasher.Hash("Agent666");

    // Track used emails to handle duplicates
    private static readonly HashSet<string> UsedEmails = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Generates email in format: {first_letter}.{lastname}@pbx.local
    /// Removes Polish diacritics and handles duplicates.
    /// </summary>
    internal static string GenerateEmail(string firstName, string lastName)
    {
        var normalizedFirst = RemoveDiacritics(firstName).ToLowerInvariant();
        var normalizedLast = RemoveDiacritics(lastName).ToLowerInvariant();

        var baseEmail = $"{normalizedFirst[0]}.{normalizedLast}@pbx.local";

        if (UsedEmails.Add(baseEmail))
            return baseEmail;

        // Handle duplicates by adding number suffix
        for (int i = 2; i <= 99; i++)
        {
            var altEmail = $"{normalizedFirst[0]}.{normalizedLast}{i}@pbx.local";
            if (UsedEmails.Add(altEmail))
                return altEmail;
        }

        // Fallback: use full first name
        var fullEmail = $"{normalizedFirst}.{normalizedLast}@pbx.local";
        UsedEmails.Add(fullEmail);
        return fullEmail;
    }

    /// <summary>
    /// Removes Polish and other diacritics from string.
    /// </summary>
    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        // Handle special Polish characters that don't decompose well
        return sb.ToString()
            .Replace("ł", "l")
            .Replace("Ł", "L")
            .Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Returns all users for seeding.
    /// </summary>
    public static List<AppUser> GetAllUsers() => GetUsers();

    /// <summary>
    /// Returns only active users.
    /// </summary>
    public static List<AppUser> GetActiveUsers() => GetUsers().Where(u => u.IsActive).ToList();
}
