using App.Shared.Web.BaseModel;
using Common.Toolkit.ResultPattern;
using Microsoft.AspNetCore.Http;

namespace App.Shared.Web.ApiResultPattern;

public static class ResultToApiMapper
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        return result switch
        {
            Success<T> success => Results.Ok(success.Value),
            NotFoundFailure<T> failure => Results.NotFound(failure.Error.ToApiError()),
            ForbiddenFailure<T> failure => Results.Forbid(),
            BusinessLogicFailure<T> failure => Results.BadRequest(failure.Error.ToApiError()),
            ValidationFailure<T> failure => Results.BadRequest(new ApiErrorResponse
            {
                Code = failure.Error.Code,
                Message = failure.Error.Message,
                ValidationErrors = failure.Error.ValidationMessages.ToList()
            }),
            GenericFailure<T> failure => Results.BadRequest(failure.Error.ToApiError()),
            _ => Results.Problem("An unexpected error occurred")
        };
    }

    private static ApiErrorResponse ToApiError(this Error error)
    {
        return new ApiErrorResponse
        {
            Code = error.Code,
            Message = error.Message
        };
    }
}