using System.Net;
using System.Net.Http.Json;
using AnswerRule.Api.Features.Rules.Model;
using AnswerRule.Api.Tests.Infrastructure;
using AnswerRule.Data.Enums;
using App.Bps.Enum;
using App.Shared.Tests;
using App.Shared.Web.BaseModel;
using Xunit;

namespace AnswerRule.Api.Tests;

[Collection(AnswerRuleDatabaseCollection.Name)]
public class RulesEndpointTests
{
    private readonly AnswerRuleMySqlTestContainer _container;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RulesEndpointTests(AnswerRuleMySqlTestContainer container)
    {
        _container = container;
    }

    #region GetList

    [Fact]
    public async Task GetList_ReturnsPagedResults()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var filter = new AnsweringRuleListFilter
        {
            SipAccountGid = AnswerRuleTestDataSeeder.TestSipAccountGid,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules/list", filter);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<PagedResult<AnsweringRuleResponse>>();
        Assert.NotNull(result);
        Assert.True(result.TotalCount >= 1);
    }

    #endregion

    #region GetByGid

    [Fact]
    public async Task GetByGid_ExistingRule_ReturnsRule()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/rules/{AnswerRuleTestDataSeeder.TestRuleGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rule = await response.ReadWithJson<AnsweringRuleDetailResponse>();
        Assert.NotNull(rule);
        Assert.Equal("Test Rule", rule.Name);
        Assert.Equal(AnsweringRuleAction.Voicemail, rule.ActionType);
        Assert.NotEmpty(rule.TimeSlots);
    }

    [Fact]
    public async Task GetByGid_NonExistingRule_Returns404()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/rules/nonexistent00000000000000001");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedRule()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "newsip000100010001000100010001",
            Name = $"New Test Rule {Guid.NewGuid():N}",
            Description = "Created by test",
            Priority = 50,
            IsEnabled = true,
            ActionType = AnsweringRuleAction.Redirect,
            ActionTarget = "+48123456789",
            SendEmailNotification = false,
            TimeSlots =
            [
                new TimeSlotDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = "09:00",
                    EndTime = "17:00",
                    IsAllDay = false
                }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var rule = await response.ReadWithJson<AnsweringRuleDetailResponse>();
        Assert.NotNull(rule);
        Assert.Contains("New Test Rule", rule.Name);
        Assert.Equal(AnsweringRuleAction.Redirect, rule.ActionType);
        Assert.Equal("+48123456789", rule.ActionTarget);
        Assert.Single(rule.TimeSlots);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010002",
            Name = "",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_NoTimeSlots_Returns400()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010003",
            Name = "Rule Without Slots",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots = []
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidTimeGranularity_Returns400()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010004",
            Name = "Rule With Invalid Time",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = "09:17", // Invalid - not multiple of 15
                    EndTime = "17:00"
                }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_OverlappingSlots_Returns400()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010005",
            Name = "Rule With Overlapping Slots",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" },
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "12:00", EndTime = "18:00" } // Overlaps
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_VoicemailWithoutBox_Returns400()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010006",
            Name = "Voicemail Without Box",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = null, // Missing required field
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_RedirectWithoutTarget_Returns400()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010007",
            Name = "Redirect Without Target",
            ActionType = AnsweringRuleAction.Redirect,
            ActionTarget = null, // Missing required field
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedRule()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // First create a rule
        var createRequest = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010010",
            Name = "Rule To Update",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };
        var createResponse = await client.PostAsJsonAsync("/api/rules", createRequest);
        var createdRule = await createResponse.ReadWithJson<AnsweringRuleDetailResponse>();

        // Update
        var updateRequest = new UpdateAnsweringRuleRequest
        {
            Name = "Updated Rule Name",
            ActionType = AnsweringRuleAction.Redirect,
            ActionTarget = "+48999888777",
            Priority = 25,
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Tuesday, StartTime = "10:00", EndTime = "18:00" }
            ]
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/rules/{createdRule!.Gid}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedRule = await response.ReadWithJson<AnsweringRuleDetailResponse>();
        Assert.NotNull(updatedRule);
        Assert.Equal("Updated Rule Name", updatedRule.Name);
        Assert.Equal(AnsweringRuleAction.Redirect, updatedRule.ActionType);
        Assert.Equal("+48999888777", updatedRule.ActionTarget);
        Assert.Equal(25, updatedRule.Priority);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ExistingRule_Returns200()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // First create a rule
        var createRequest = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010020",
            Name = "Rule To Delete",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };
        var createResponse = await client.PostAsJsonAsync("/api/rules", createRequest);
        var createdRule = await createResponse.ReadWithJson<AnsweringRuleDetailResponse>();

        // Act
        var response = await client.DeleteAsync($"/api/rules/{createdRule!.Gid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify it's deleted (soft delete)
        var getResponse = await client.GetAsync($"/api/rules/{createdRule.Gid}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    #endregion

    #region Toggle

    [Fact]
    public async Task Toggle_TogglesIsEnabled()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // First create a rule (enabled by default)
        var createRequest = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "testsip00100010001000100010030",
            Name = "Rule To Toggle",
            IsEnabled = true,
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };
        var createResponse = await client.PostAsJsonAsync("/api/rules", createRequest);
        var createdRule = await createResponse.ReadWithJson<AnsweringRuleDetailResponse>();
        Assert.True(createdRule!.IsEnabled);

        // Act - toggle off
        var response = await client.PatchAsync($"/api/rules/{createdRule.Gid}/toggle", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var toggledRule = await response.ReadWithJson<AnsweringRuleDetailResponse>();
        Assert.NotNull(toggledRule);
        Assert.False(toggledRule.IsEnabled);

        // Toggle back on
        var response2 = await client.PatchAsync($"/api/rules/{createdRule.Gid}/toggle", null);
        var toggledRule2 = await response2.ReadWithJson<AnsweringRuleDetailResponse>();
        Assert.True(toggledRule2!.IsEnabled);
    }

    #endregion

    #region CheckActiveRule

    [Fact]
    public async Task CheckActiveRule_MatchingRule_ReturnsRule()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // Test rule has Monday 17:00-23:00 and Saturday all day
        var request = new CheckRuleRequest
        {
            SipAccountGid = AnswerRuleTestDataSeeder.TestSipAccountGid,
            CheckDateTime = GetNextDayOfWeek(DayOfWeek.Monday).AddHours(18) // Monday 18:00
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/check", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<CheckRuleResponse>();
        Assert.NotNull(result);
        Assert.True(result.HasActiveRule);
        Assert.NotNull(result.Rule);
        Assert.Equal("Test Rule", result.Rule.Name);
    }

    [Fact]
    public async Task CheckActiveRule_NoMatchingRule_ReturnsNoRule()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // Test rule has Monday 17:00-23:00, so 10:00 should not match
        var request = new CheckRuleRequest
        {
            SipAccountGid = AnswerRuleTestDataSeeder.TestSipAccountGid,
            CheckDateTime = GetNextDayOfWeek(DayOfWeek.Monday).AddHours(10) // Monday 10:00
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/check", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CheckRuleResponse>();
        Assert.NotNull(result);
        Assert.False(result.HasActiveRule);
        Assert.Null(result.Rule);
    }

    [Fact]
    public async Task CheckActiveRule_PriorityOrder_ReturnsHighestPriority()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.Root]);
        using var client = factory.CreateClient();

        // Create two rules with different priorities for the same time
        var lowPriorityRequest = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "prioritytest0001000100010001",
            Name = "Low Priority Rule",
            Priority = 100,
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Wednesday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };
        await client.PostAsJsonAsync("/api/rules", lowPriorityRequest);

        var highPriorityRequest = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "prioritytest0001000100010001",
            Name = "High Priority Rule",
            Priority = 10, // Lower number = higher priority
            ActionType = AnsweringRuleAction.Redirect,
            ActionTarget = "+48111222333",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Wednesday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };
        await client.PostAsJsonAsync("/api/rules", highPriorityRequest);

        // Check
        var checkRequest = new CheckRuleRequest
        {
            SipAccountGid = "prioritytest0001000100010001",
            CheckDateTime = GetNextDayOfWeek(DayOfWeek.Wednesday).AddHours(12) // Wednesday 12:00
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/check", checkRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.ReadWithJson<CheckRuleResponse>();
        Assert.NotNull(result);
        Assert.True(result.HasActiveRule);
        Assert.Equal("High Priority Rule", result.Rule!.Name);
    }

    #endregion

    #region Authorization

    [Theory]
    [InlineData(Roles.Root)]
    [InlineData(Roles.Ops)]
    [InlineData(Roles.Admin)]
    public async Task Create_AllowedForAdminRoles(string role)
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [role]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = $"auth{role}00100010001000100010001",
            Name = $"Auth Test Rule {role}",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_ForbiddenForUserRole()
    {
        // Arrange
        using var factory = new AnswerRuleApplicationFactory(
            _container.ConnectionString,
            options => options.Roles = [Roles.User]);
        using var client = factory.CreateClient();

        var request = new CreateAnsweringRuleRequest
        {
            SipAccountGid = "authuser0010001000100010001001",
            Name = "User Should Not Create",
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = "vm001",
            TimeSlots =
            [
                new TimeSlotDto { DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" }
            ]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/rules", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    private static DateTime GetNextDayOfWeek(DayOfWeek dayOfWeek)
    {
        var today = DateTime.UtcNow.Date;
        var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7; // Always get next week's day
        return today.AddDays(daysUntil);
    }
}
