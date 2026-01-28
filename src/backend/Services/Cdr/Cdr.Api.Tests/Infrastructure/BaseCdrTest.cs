using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;

namespace CdrService.Api.Tests.Infrastructure;

public abstract class BaseCdrTest
{
    protected void ConfigureAuthForFullAccess(TestAuthHandlerOptions options)
    {
        options.Email = "test@fake.mail";
        options.Roles = [nameof(AppRole.Root), nameof(AppRole.Ops)];
        options.FirstName = "Test";
        options.LastName = "Test";
    }
}
