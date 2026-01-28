using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;
using RateService.Api.Features.Tariffs.Model;
using RateService.Api.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace RateService.Api.Tests;

[Collection(RateServiceDatabaseCollection.Name)]
public class TariffsEndpointTests : BaseRateTest, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly RateServiceApplicationFactory _factory;
    private readonly HttpClient _client;

    public TariffsEndpointTests(RateServiceMySqlTestContainer container, ITestOutputHelper output)
    {
        _output = output;

        _factory = new RateServiceApplicationFactory(container.ConnectionString, ConfigureAuthForFullAccess);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetList_ReturnsTariffs()
    {
        // Arrange
        var filter = new TariffListFilter { PageNumber = 1, PageSize = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tariffs/list", filter);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByGid_ExistingTariff_ReturnsTariff()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/tariffs/{RateServiceTestDataSeeder.TestTariffGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tariff = await response.Content.ReadFromJsonAsync<TariffDetailResponse>();
        Assert.NotNull(tariff);
        Assert.Equal("Test Tariff", tariff.Name);
    }

    [Fact]
    public async Task GetByGid_NonExistingTariff_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/tariffs/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedTariff()
    {
        // Arrange
        var request = new CreateTariffRequest
        {
            Name = $"New Tariff {Guid.NewGuid():N}",
            Description = "New tariff description",
            CurrencyCode = "EUR",
            BillingIncrement = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tariffs", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tariff = await response.Content.ReadFromJsonAsync<TariffResponse>();
        Assert.NotNull(tariff);
        Assert.Contains("New Tariff", tariff.Name);
        Assert.Equal("EUR", tariff.CurrencyCode);
        Assert.NotEmpty(tariff.Gid);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400()
    {
        // Arrange
        var request = new CreateTariffRequest
        {
            Name = "",
            CurrencyCode = "PLN"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tariffs", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedTariff()
    {
        // Arrange
        var request = new UpdateTariffRequest
        {
            Name = "Updated Tariff",
            CurrencyCode = "PLN",
            IsActive = true,
            BillingIncrement = 60
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/tariffs/{RateServiceTestDataSeeder.TestTariffGid}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tariff = await response.Content.ReadFromJsonAsync<TariffResponse>();
        Assert.NotNull(tariff);
        Assert.Equal("Updated Tariff", tariff.Name);
    }

    [Fact]
    public async Task Delete_NonDefaultTariff_ReturnsSuccess()
    {
        // Arrange - najpierw utwórz taryfę do usunięcia
        var createRequest = new CreateTariffRequest
        {
            Name = $"To Delete {Guid.NewGuid():N}",
            CurrencyCode = "PLN",
            IsDefault = false,
            BillingIncrement = 60
        };
        var createResponse = await _client.PostAsJsonAsync("/api/tariffs", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TariffResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/tariffs/{created!.Gid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
