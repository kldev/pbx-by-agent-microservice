using Xunit;

namespace Rcp.Api.Tests.Infrastructure;

/// <summary>
/// xUnit collection fixture for sharing MySQL container across test classes.
/// All test classes using [Collection(Name)] will share the same database container.
/// </summary>
[CollectionDefinition(Name)]
public class RcpDatabaseCollection : ICollectionFixture<RcpMySqlTestContainer>
{
    public const string Name = "RcpDatabase";
}
