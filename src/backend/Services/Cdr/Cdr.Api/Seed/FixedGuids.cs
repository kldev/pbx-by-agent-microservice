namespace CdrService.Api.Seed;

public static class FixedGuids
{
    // Call Statuses
    public const string StatusCompleted = "cs000001000000000000000000000001";
    public const string StatusFailed = "cs000001000000000000000000000002";
    public const string StatusBusy = "cs000001000000000000000000000003";
    public const string StatusNoAnswer = "cs000001000000000000000000000004";
    public const string StatusCancelled = "cs000001000000000000000000000005";
    public const string StatusRinging = "cs000001000000000000000000000006";

    // Termination Causes
    public const string CauseNormalClearing = "tc000001000000000000000000000001";
    public const string CauseUserBusy = "tc000001000000000000000000000002";
    public const string CauseNoAnswer = "tc000001000000000000000000000003";
    public const string CauseCallRejected = "tc000001000000000000000000000004";
    public const string CauseNumberChanged = "tc000001000000000000000000000005";
    public const string CauseDestinationOutOfOrder = "tc000001000000000000000000000006";
    public const string CauseInvalidNumberFormat = "tc000001000000000000000000000007";
    public const string CauseNetworkOutOfOrder = "tc000001000000000000000000000008";
    public const string CauseTemporaryFailure = "tc000001000000000000000000000009";
    public const string CauseCongestion = "tc000001000000000000000000000010";

    // Call Directions
    public const string DirectionInbound = "cd000001000000000000000000000001";
    public const string DirectionOutbound = "cd000001000000000000000000000002";
    public const string DirectionInternal = "cd000001000000000000000000000003";

    // Sample Call Records (for showcase)
    public const string CallRecordSample1 = "cr000001000000000000000000000001";
    public const string CallRecordSample2 = "cr000001000000000000000000000002";
}
