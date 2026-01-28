using Xunit;

namespace Identity.Api.Tests.Infrastructure;

/// <summary>
/// xUnit collection fixture for sharing MySQL container across test classes.
/// All test classes using [Collection(Name)] will share the same database container.
/// </summary>
[CollectionDefinition(Name)]
public class IdentityDatabaseCollection : ICollectionFixture<IdentityMySqlTestContainer>
{
    public const string Name = "IdentityDatabase";
}
