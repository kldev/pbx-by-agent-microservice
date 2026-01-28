using Rcp.Data;

namespace Rcp.Api.Tests.Infrastructure;

/// <summary>
/// Seeds test data for RCP service integration tests.
/// Tests create data dynamically via API, so minimal seeding is needed.
/// </summary>
public static class RcpTestDataSeeder
{
    public static void SeedTestData(RcpDbContext context)
    {
        // RCP tests create entries dynamically via API.
        // No seed data required - just ensure clean state.
    }
}
