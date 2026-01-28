namespace Common.Toolkit.ResultPattern;

public abstract class Result
{
    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Success Success() => new Success();
    public static Failure Failure(Error error) => new Failure(error);
}

public class Success : Result { }

public class Failure : Result
{
    public Error Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public Failure(Error error) => Error = error;
}

public abstract class Result<T>
{
    public bool IsSuccess => this is Success<T>;
    public bool IsFailure => this is Failure<T> or ValidationFailure<T> or GenericFailure<T> or BusinessLogicFailure<T> or NotFoundFailure<T> or ForbiddenFailure<T>;

    public static Success<T> Success(T value) => new(value);

    public static ValidationFailure<T> Failure(ValidationError error) => new(error);
    public static GenericFailure<T> Failure(GenericError error) => new(error);
    public static BusinessLogicFailure<T> Failure(BusinessLogicError error) => new(error);
    public static NotFoundFailure<T> Failure(NotFoundError error) => new(error);
    public static ForbiddenFailure<T> Failure(ForbiddenError error) => new(error);

    public static NotFoundFailure<T> NotFoundFailure(GenericError error) => new(error);

    /// <summary>
    /// Pattern match na Result - wykonuje odpowiednią funkcję w zależności od sukcesu/błędu
    /// </summary>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return this switch
        {
            Success<T> s => onSuccess(s.Value),
            ValidationFailure<T> f => onFailure(f.Error),
            NotFoundFailure<T> f => onFailure(f.Error),
            ForbiddenFailure<T> f => onFailure(f.Error),
            BusinessLogicFailure<T> f => onFailure(f.Error),
            GenericFailure<T> f => onFailure(f.Error),
            Failure<T> f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };
    }

    /// <summary>
    /// Mapuje wartość sukcesu na nowy typ, zachowując błędy
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return this switch
        {
            Success<T> s => Result<TNew>.Success(mapper(s.Value)),
            ValidationFailure<T> f => Result<TNew>.Failure(f.Error),
            NotFoundFailure<T> f => Result<TNew>.Failure(f.Error),
            ForbiddenFailure<T> f => Result<TNew>.Failure(f.Error),
            BusinessLogicFailure<T> f => Result<TNew>.Failure(f.Error),
            GenericFailure<T> f => Result<TNew>.Failure(f.Error),
            Failure<T> f => Result<TNew>.Failure(new GenericError(f.Error.Code, f.Error.Message)),
            _ => throw new InvalidOperationException("Unknown Result type")
        };
    }

    /// <summary>
    /// Wykonuje akcję na wartości sukcesu (side effect), zwraca oryginalny Result
    /// </summary>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (this is Success<T> s)
            action(s.Value);
        return this;
    }

    /// <summary>
    /// Wykonuje akcję na błędzie (side effect), zwraca oryginalny Result
    /// </summary>
    public Result<T> OnFailure(Action<Error> action)
    {
        var error = this switch
        {
            ValidationFailure<T> f => f.Error,
            NotFoundFailure<T> f => f.Error,
            ForbiddenFailure<T> f => f.Error,
            BusinessLogicFailure<T> f => f.Error,
            GenericFailure<T> f => f.Error,
            Failure<T> f => f.Error,
            _ => null
        };
        if (error != null)
            action(error);
        return this;
    }
}

public class Success<T> : Result<T>
{
    public T Value { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public Success(T value) => Value = value;
}

public class Failure<T> : Result<T>
{
    public Error Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public Failure(Error error) => Error = error;
}

public class ValidationFailure<T> : Result<T>
{
    public ValidationError Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public ValidationFailure(ValidationError error) => Error = error;
}

public class GenericFailure<T> : Result<T>
{
    public GenericError Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public GenericFailure(GenericError error) => Error = error;
}

public class NotFoundFailure<T> : Result<T>
{
    public NotFoundError Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public NotFoundFailure(NotFoundError error) => Error = error;

    // Legacy constructor for backwards compatibility
    public NotFoundFailure(GenericError error) => Error = new NotFoundError(error.Code, error.Message, error.Details, error.Exception);
}

public class BusinessLogicFailure<T> : Result<T>
{
    public BusinessLogicError Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public BusinessLogicFailure(BusinessLogicError error) => Error = error;
}

public class ForbiddenFailure<T> : Result<T>
{
    public ForbiddenError Error { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public ForbiddenFailure(ForbiddenError error) => Error = error;
}
