using Xunit;

namespace CdrService.Api.Tests.Infrastructure;

[CollectionDefinition(Name)]
public class CdrDatabaseCollection : ICollectionFixture<CdrMySqlTestContainer>
{
    public const string Name = "CdrServiceDatabase";
}
