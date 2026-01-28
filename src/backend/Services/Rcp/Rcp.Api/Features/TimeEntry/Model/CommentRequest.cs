namespace Rcp.Api.Features.TimeEntry.Model;

public record CommentRequest(string Content);

public record ChangeStatusRequest(string? Comment);
