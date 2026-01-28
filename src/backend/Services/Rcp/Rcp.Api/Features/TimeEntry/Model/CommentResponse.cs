namespace Rcp.Api.Features.TimeEntry.Model;

public record CommentResponse(
    string Gid,
    string Content,
    string? AuthorName,
    string? AuthorRole,
    DateTime CreatedAt
);
