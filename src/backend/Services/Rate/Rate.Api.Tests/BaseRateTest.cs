using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;

namespace RateService.Api.Tests;

public abstract class BaseRateTest
{
    protected void ConfigureAuthForFullAccess(TestAuthHandlerOptions _)
    {
        _.Email = "test@fake.mail";
        _.Roles = [nameof(AppRole.Root), nameof(AppRole.Ops)];
        _.FirstName = "Test";
        _.LastName = "Test";
    }
}
