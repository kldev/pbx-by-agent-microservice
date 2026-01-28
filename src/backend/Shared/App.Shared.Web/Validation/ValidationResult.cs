using Common.Toolkit.ResultPattern;

namespace App.Shared.Web.Validation;

public class ValidationResult
{
    public bool IsValid { get; init; } = true;
    public List<string> Errors { get; init; } = new();

    /// <summary>
    /// Returns true if there are any validation errors
    /// </summary>
    public bool HasErrors => !IsValid || Errors.Count > 0;

    public static ValidationResult Success() => new() { IsValid = true };

    public static ValidationResult Failure(string error) => new()
    {
        IsValid = false,
        Errors = new List<string> { error }
    };

    public static ValidationResult Failure(IEnumerable<string> errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };

    /// <summary>
    /// Converts validation errors to a Result failure
    /// </summary>
    public Result<T> ToFailure<T>()
    {
        var errorMessage = string.Join("; ", Errors);
        return Result<T>.Failure(new ValidationError("validation.failed", errorMessage));
    }
}

/// <summary>
/// Fluent validator for request objects
/// </summary>
public class RequestValidator<T>
{
    private readonly T _request;
    private readonly List<string> _errors = new();

    private RequestValidator(T request)
    {
        _request = request;
    }

    public static RequestValidator<T> For(T request) => new(request);

    public RequestValidator<T> AddError(string error)
    {
        _errors.Add(error);
        return this;
    }

    public RequestValidator<T> AddErrorIf(bool condition, string error)
    {
        if (condition)
            _errors.Add(error);
        return this;
    }

    /// <summary>
    /// Validates that a string property is not null or empty
    /// </summary>
    public RequestValidator<T> Required(System.Linq.Expressions.Expression<Func<T, string?>> selector, string fieldName)
    {
        var func = selector.Compile();
        var value = func(_request);
        if (string.IsNullOrWhiteSpace(value))
            _errors.Add($"Pole '{fieldName}' jest wymagane");
        return this;
    }

    /// <summary>
    /// Validates that a string property doesn't exceed max length
    /// </summary>
    public RequestValidator<T> MaxLength(System.Linq.Expressions.Expression<Func<T, string?>> selector, int maxLength, string fieldName)
    {
        var func = selector.Compile();
        var value = func(_request);
        if (value != null && value.Length > maxLength)
            _errors.Add($"Pole '{fieldName}' nie może przekraczać {maxLength} znaków");
        return this;
    }

    /// <summary>
    /// Validates that a numeric value is greater than a minimum
    /// </summary>
    public RequestValidator<T> Min<TValue>(System.Linq.Expressions.Expression<Func<T, TValue>> selector, TValue min, string fieldName) where TValue : IComparable<TValue>
    {
        var func = selector.Compile();
        var value = func(_request);
        if (value != null && value.CompareTo(min) < 0)
            _errors.Add($"Pole '{fieldName}' musi być większe lub równe {min}");
        return this;
    }

    /// <summary>
    /// Validates that a numeric value is less than a maximum
    /// </summary>
    public RequestValidator<T> Max<TValue>(System.Linq.Expressions.Expression<Func<T, TValue>> selector, TValue max, string fieldName) where TValue : IComparable<TValue>
    {
        var func = selector.Compile();
        var value = func(_request);
        if (value != null && value.CompareTo(max) > 0)
            _errors.Add($"Pole '{fieldName}' musi być mniejsze lub równe {max}");
        return this;
    }

    public ValidationResult Validate()
    {
        if (_errors.Count == 0)
            return ValidationResult.Success();

        return ValidationResult.Failure(_errors);
    }
}
