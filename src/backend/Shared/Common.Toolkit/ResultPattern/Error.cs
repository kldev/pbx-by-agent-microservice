namespace Common.Toolkit.ResultPattern;

public abstract class Error
{
    public string Code { get; } = "";
    public string Message { get; } = "";
    public string Details { get; } = "";
    public Exception? Exception { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    protected Error(string code, string message, string details = "", Exception? exception = null)
    {
        Code = code;
        Message = message;
        Details = details;
        Exception = exception;
    }

    public override string ToString() => $"{Code}: {Message}. Details: {Details}";
}

public class ValidationError : Error
{
    public IEnumerable<string> ValidationMessages { get; }

    public ValidationError()
        : base("form.validation", "Request invalid", "")
        => ValidationMessages = [];

    public ValidationError(string code, string message)
        : base("form.validation", "Request invalid", "")
        => ValidationMessages = new[] { code, message };

    public ValidationError(IEnumerable<string> validationMessages)
        : base("form.validation", "Request invalid", "") => ValidationMessages = validationMessages;
}

public class BusinessLogicError : Error
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public BusinessLogicError(string code, string message, string details = "", Exception? exception = null)
        : base(code, message, details, exception) { }
}

public class NotFoundError : Error
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public NotFoundError(string code, string message, string details = "", Exception? exception = null)
        : base(code, message, details, exception) { }
}

public class ForbiddenError : Error
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public ForbiddenError(string code, string message, string details = "", Exception? exception = null)
        : base(code, message, details, exception) { }
}

public class GenericError : Error
{
    public GenericError() : base("", "", "") { }

    // ReSharper disable once ConvertToPrimaryConstructor
    public GenericError(string code, string message) : base(code, message, "") { }

    public GenericError(string code, string message, string details, Exception? exception = null)
        : base(code, message, details, exception) { }
}
