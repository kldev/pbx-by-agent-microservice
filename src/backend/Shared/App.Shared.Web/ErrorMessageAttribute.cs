namespace App.Shared.Web;

/// <summary>
/// Attribute to define error message translations for error codes
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ErrorMessageAttribute : Attribute
{
    /// <summary>
    /// Polish translation of the error message
    /// </summary>
    public string Polish { get; set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public ErrorMessageAttribute(string polish)
    {
        Polish = polish;
    }
}
