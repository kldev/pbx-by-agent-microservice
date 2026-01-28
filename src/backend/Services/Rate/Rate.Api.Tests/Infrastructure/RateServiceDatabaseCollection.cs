using Xunit;

namespace RateService.Api.Tests.Infrastructure;

[CollectionDefinition(Name)]
public class RateServiceDatabaseCollection : ICollectionFixture<RateServiceMySqlTestContainer>
{
    public const string Name = "RateServiceDatabase";
}
