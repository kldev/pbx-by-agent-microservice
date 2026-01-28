using Xunit;

namespace DataSource.Api.Tests.Infrastructure;

/// <summary>
/// Collection definition for DataSource integration tests.
/// All test classes using [Collection(Name)] will share the same database container.
/// </summary>
[CollectionDefinition(Name)]
public class DataSourceDatabaseCollection : ICollectionFixture<AllDatabasesTestContainer>
{
    public const string Name = "DataSourceDatabase";
}
