using Xunit;

namespace AnswerRule.Api.Tests.Infrastructure;

[CollectionDefinition(Name)]
public class AnswerRuleDatabaseCollection : ICollectionFixture<AnswerRuleMySqlTestContainer>
{
    public const string Name = "AnswerRuleDatabase";
}
