using Testcontainers.MySql;
using Xunit;

namespace App.Shared.Tests.Infrastructure;

/// <summary>
/// Base MySQL test container that can be shared across test classes.
/// Each service should inherit from this class and implement database-specific initialization.
/// </summary>
public abstract class MySqlTestContainerBase : IAsyncLifetime
{
    private readonly MySqlContainer _container;

    public string ConnectionString { get; private set; } = string.Empty;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    protected MySqlTestContainerBase()
    {
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("Test@123!")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        await InitializeDatabaseAsync();
    }

    /// <summary>
    /// Override this method to run migrations and seed test data
    /// </summary>
    protected virtual Task InitializeDatabaseAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}
