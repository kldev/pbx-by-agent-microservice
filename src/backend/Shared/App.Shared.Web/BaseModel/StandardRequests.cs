namespace App.Shared.Web.BaseModel;


public record GetByIdRequest { public long Id { get; init; } }
public record GetByGidRequest { public string Gid { get; init; } = string.Empty; }
public record DeleteByIdRequest { public long Id { get; init; } }
public record DeleteByGidRequest { public string Gid { get; init; } = string.Empty; }

// Parent-scoped list requests
public record GetByPositionGidRequest { public string PositionGid { get; init; } = string.Empty; }
public record GetByProjectGidRequest { public string ProjectGid { get; init; } = string.Empty; }
public record GetByAssignmentGidRequest { public string AssignmentGid { get; init; } = string.Empty; }