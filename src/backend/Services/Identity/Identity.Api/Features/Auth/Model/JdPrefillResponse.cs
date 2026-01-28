namespace Identity.Api.Features.Auth.Model;

/// <summary>
/// Response for JD form prefill - returns current user's data for pre-filling JD creation form
/// </summary>
public record JdPrefillResponse(
    // Sales person (current user)
    long SalesId,
    string SalesGid,
    string SalesName,

    // SBU
    int StructureId,
    string SbuName,

    // Team (optional)
    long? TeamId,
    string? TeamGid,
    string? TeamName
);
