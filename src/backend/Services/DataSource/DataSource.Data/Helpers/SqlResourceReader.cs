using System.Reflection;

namespace DataSource.Data.Helpers;

public static class SqlResourceReader
{
    private static readonly Assembly Assembly = typeof(SqlResourceReader).Assembly;
    private const string ResourcePrefix = "DataSource.Data.Resources.Views.";

    /// <summary>
    /// Reads SQL script from embedded resources
    /// </summary>
    /// <param name="fileName">File name e.g. "vw_countries.sql"</param>
    public static string ReadSql(string fileName)
    {
        var resourceName = $"{ResourcePrefix}{fileName}";

        using var stream = Assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Reads UP and DOWN scripts for a view
    /// </summary>
    public static (string Up, string Down) ReadViewSql(string viewName)
    {
        var up = ReadSql($"{viewName}.sql");
        var down = $"DROP VIEW IF EXISTS {viewName};";
        return (up, down);
    }
}
