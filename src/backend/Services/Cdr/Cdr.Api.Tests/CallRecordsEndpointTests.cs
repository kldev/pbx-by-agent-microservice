using System.Net;
using System.Net.Http.Json;
using CdrService.Api.Features.CallRecords.Model;
using CdrService.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CdrService.Api.Tests;

[Collection(CdrDatabaseCollection.Name)]
public class CallRecordsEndpointTests : BaseCdrTest, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly CdrApplicationFactory _factory;
    private readonly HttpClient _client;

    public CallRecordsEndpointTests(CdrMySqlTestContainer container, ITestOutputHelper output)
    {
        _output = output;
        _factory = new CdrApplicationFactory(container.ConnectionString, ConfigureAuthForFullAccess);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetList_ReturnsCallRecords()
    {
        // Arrange
        var filter = new CallRecordListFilter { PageNumber = 1, PageSize = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/call-records/list", filter);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetList_WithDateFilter_ReturnsFilteredResults()
    {
        // Arrange
        var filter = new CallRecordListFilter
        {
            PageNumber = 1,
            PageSize = 10,
            StartDateFrom = DateTime.UtcNow.AddDays(-1),
            StartDateTo = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/call-records/list", filter);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByGid_ExistingRecord_ReturnsRecord()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/call-records/{CdrTestDataSeeder.TestCallRecordGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var record = await response.Content.ReadFromJsonAsync<CallRecordDetailResponse>();
        Assert.NotNull(record);
        Assert.Equal("+48501111111", record.CallerId);
        Assert.Equal("+48502222222", record.CalledNumber);
    }

    [Fact]
    public async Task GetByGid_NonExistingRecord_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/call-records/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedRecord()
    {
        // Arrange
        var request = new CreateCallRecordRequest
        {
            CallerId = "+48601234567",
            CalledNumber = "+48609876543",
            StartTime = DateTime.UtcNow.AddMinutes(-10),
            EndTime = DateTime.UtcNow.AddMinutes(-5),
            Duration = 300,
            BillableSeconds = 300,
            CallStatusId = 1, // Completed
            TerminationCauseId = 1, // Normal Clearing
            CallDirectionId = 2, // Outbound
            TariffName = "Test",
            RatePerMinute = 0.15m,
            BillingIncrement = 60,
            CurrencyCode = "PLN",
            TotalCost = 0.75m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/call-records", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var record = await response.Content.ReadFromJsonAsync<CallRecordResponse>();
        Assert.NotNull(record);
        Assert.Equal("+48601234567", record.CallerId);
        Assert.NotEmpty(record.Gid);
    }

    [Fact]
    public async Task Create_MissingCallerId_Returns400()
    {
        // Arrange
        var request = new CreateCallRecordRequest
        {
            CallerId = "",
            CalledNumber = "+48609876543",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(5),
            Duration = 300,
            BillableSeconds = 300,
            CallStatusId = 1,
            TerminationCauseId = 1,
            CallDirectionId = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/call-records", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_EndTimeBeforeStartTime_Returns400()
    {
        // Arrange
        var request = new CreateCallRecordRequest
        {
            CallerId = "+48601234567",
            CalledNumber = "+48609876543",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(-5), // Before start
            Duration = 300,
            BillableSeconds = 300,
            CallStatusId = 1,
            TerminationCauseId = 1,
            CallDirectionId = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/call-records", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidCallStatus_Returns400()
    {
        // Arrange
        var request = new CreateCallRecordRequest
        {
            CallerId = "+48601234567",
            CalledNumber = "+48609876543",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(5),
            Duration = 300,
            BillableSeconds = 300,
            CallStatusId = 999, // Invalid
            TerminationCauseId = 1,
            CallDirectionId = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/call-records", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedRecord()
    {
        // Arrange
        var request = new UpdateCallRecordRequest
        {
            EndTime = DateTime.UtcNow,
            Duration = 200,
            BillableSeconds = 180,
            CallStatusId = 1,
            TerminationCauseId = 1,
            TotalCost = 0.30m,
            UserData = "Updated via test"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/call-records/{CdrTestDataSeeder.TestCallRecordGid}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var record = await response.Content.ReadFromJsonAsync<CallRecordResponse>();
        Assert.NotNull(record);
        Assert.Equal(0.30m, record.TotalCost);
    }

    [Fact]
    public async Task Update_NonExistingRecord_Returns404()
    {
        // Arrange
        var request = new UpdateCallRecordRequest
        {
            EndTime = DateTime.UtcNow,
            Duration = 200,
            BillableSeconds = 180,
            CallStatusId = 1,
            TerminationCauseId = 1,
            TotalCost = 0.30m
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/call-records/non-existing-gid", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRecord_ReturnsSuccess()
    {
        // Arrange - create record to delete
        var createRequest = new CreateCallRecordRequest
        {
            CallerId = "+48701234567",
            CalledNumber = "+48709876543",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(1),
            Duration = 60,
            BillableSeconds = 60,
            CallStatusId = 1,
            TerminationCauseId = 1,
            CallDirectionId = 2
        };
        var createResponse = await _client.PostAsJsonAsync("/api/call-records", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CallRecordResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/call-records/{created!.Gid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistingRecord_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/call-records/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
