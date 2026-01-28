using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;
using RateService.Api.Features.Rates.Model;
using RateService.Api.Tests.Infrastructure;
using Xunit;

namespace RateService.Api.Tests;

[Collection(RateServiceDatabaseCollection.Name)]
public class RatesEndpointTests : BaseRateTest, IAsyncLifetime
{
    private readonly RateServiceApplicationFactory _factory;
    private readonly HttpClient _client;

    public RatesEndpointTests(RateServiceMySqlTestContainer container)
    {

        _factory = new RateServiceApplicationFactory(container.ConnectionString, ConfigureAuthForFullAccess);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetList_ReturnsRates()
    {
        // Arrange
        var filter = new RateListFilter { PageNumber = 1, PageSize = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rates/list", filter);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByGid_ExistingRate_ReturnsRate()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/rates/{RateServiceTestDataSeeder.TestRateGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rate = await response.Content.ReadFromJsonAsync<RateResponse>();
        Assert.NotNull(rate);
        Assert.Equal("+48", rate.Prefix);
    }

    [Fact]
    public async Task GetByGid_NonExistingRate_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/rates/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedRate()
    {
        // Arrange
        var request = new CreateRateRequest
        {
            TariffGid = RateServiceTestDataSeeder.TestTariffGid,
            Prefix = "+49",
            DestinationName = "Germany",
            RatePerMinute = 0.25m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rates", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rate = await response.Content.ReadFromJsonAsync<RateResponse>();
        Assert.NotNull(rate);
        Assert.Equal("+49", rate.Prefix);
        Assert.Equal(0.25m, rate.RatePerMinute);
    }

    [Fact]
    public async Task Create_DuplicatePrefix_Returns400()
    {
        // Arrange - prefiks +48 już istnieje z seed
        var request = new CreateRateRequest
        {
            TariffGid = RateServiceTestDataSeeder.TestTariffGid,
            Prefix = "+48",
            DestinationName = "Poland Duplicate",
            RatePerMinute = 0.20m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rates", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LookupRate_ExistingPrefix_ReturnsMatchingRate()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/lookup?tariffGid={RateServiceTestDataSeeder.TestTariffGid}&phoneNumber=+48123456789");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rate = await response.Content.ReadFromJsonAsync<RateResponse>();
        Assert.NotNull(rate);
        Assert.Equal("+48", rate.Prefix);
    }

    [Fact]
    public async Task LookupRate_NonMatchingPrefix_Returns404()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/lookup?tariffGid={RateServiceTestDataSeeder.TestTariffGid}&phoneNumber=+33123456789");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedRate()
    {
        // Arrange
        var updateRequest = new UpdateRateRequest
        {
            Prefix = "+48",
            DestinationName = "Poland Updated",
            RatePerMinute = 0.12m,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/{RateServiceTestDataSeeder.TestRateGid}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rate = await response.Content.ReadFromJsonAsync<RateResponse>();
        Assert.NotNull(rate);
        Assert.Equal("Poland Updated", rate.DestinationName);
        Assert.Equal(0.12m, rate.RatePerMinute);
    }

    [Fact]
    public async Task Delete_ExistingRate_ReturnsSuccess()
    {
        // Arrange - najpierw utwórz stawkę do usunięcia
        var createRequest = new CreateRateRequest
        {
            TariffGid = RateServiceTestDataSeeder.TestTariffGid,
            Prefix = "+99",
            DestinationName = "To Delete",
            RatePerMinute = 0.50m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/rates", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RateResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/rates/{created!.Gid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
