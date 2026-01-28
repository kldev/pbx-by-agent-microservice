using Rcp.Api.Infrastructure;

namespace Rcp.Api.Tests.Infrastructure;

/// <summary>
/// Mock DataSource client for integration tests.
/// Returns test user as subordinate of any supervisor (simulates supervisor access).
/// </summary>
public class MockDataSourceClient : IDataSourceClient
{
    private readonly long _testUserId;

    public MockDataSourceClient(long testUserId = 1)
    {
        _testUserId = testUserId;
    }

    public Task<List<long>> GetSubordinateIdsAsync(long supervisorId)
    {
        // In tests, supervisor (admin) can see test user entries
        return Task.FromResult(new List<long> { _testUserId });
    }
}
