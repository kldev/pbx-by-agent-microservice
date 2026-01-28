namespace App.Bps.Enum;

/// <summary>
/// Extension methods for AppRole enum to work with strings.
/// </summary>
public static class AppRoleExtensions
{
    /// <summary>
    /// Converts AppRole to string name.
    /// </summary>
    public static string ToRoleName(this AppRole role) => role.ToString();

    /// <summary>
    /// Tries to parse string to AppRole.
    /// </summary>
    public static bool TryParse(string value, out AppRole role) =>
        System.Enum.TryParse(value, ignoreCase: true, out role);

    /// <summary>
    /// Parses string to AppRole, throws if invalid.
    /// </summary>
    public static AppRole Parse(string value) =>
        System.Enum.Parse<AppRole>(value, ignoreCase: true);

    /// <summary>
    /// Checks if string is a valid AppRole name.
    /// </summary>
    public static bool IsValidRole(string value) =>
        System.Enum.TryParse<AppRole>(value, ignoreCase: true, out _);
}

/// <summary>
/// Static role name constants for use in attributes, tests, etc.
/// </summary>
public static class Roles
{
    public const string Root = nameof(AppRole.Root);
    public const string Ops = nameof(AppRole.Ops);
    public const string Admin = nameof(AppRole.Admin);
    public const string Support = nameof(AppRole.Support);
    public const string User = nameof(AppRole.User);

    /// <summary>
    /// All role names as array.
    /// </summary>
    public static readonly string[] All = [Root, Ops, Admin, Support, User];
}
