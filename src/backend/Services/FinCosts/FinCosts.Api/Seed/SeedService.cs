using FinCosts.Data;

namespace FinCosts.Api.Seed;

public class SeedService : ISeedService
{
    private readonly FinCostsDbContext _context;
    private readonly ILogger<SeedService> _logger;
    private readonly IConfiguration _configuration;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public SeedService(FinCostsDbContext context, ILogger<SeedService> logger,  IConfiguration configuration)
    {
        _configuration = configuration;
        _context = context;
        _logger = logger;
    }
    
    public async Task SeedAsync()
    {
        var seedConfig = _configuration.GetSection("Seed");

        if (!seedConfig.GetValue<bool>("RunOnStartup"))
        {
            _logger.LogInformation("Seed disabled in configuration");
            return;
        }

        _logger.LogInformation("Starting database seed...");
        await SeedDictionariesAsync();
        
        // Tariffs & Rates (jeśli showcase włączony)
        if (seedConfig.GetValue<bool>("IncludeShowcaseData"))
        {
            await SeedShowcaseDataAsync();
        }

    }
    
    public async Task SeedDictionariesAsync()
    {
      
        _logger.LogInformation("Starting dictionary seed...");

        await DictionarySeedData.SeedAsync(_context);

        _logger.LogInformation("Dictionary seed completed");
    }

    public async Task SeedShowcaseDataAsync()
    {
        _logger.LogInformation("Starting showcase data seed...");
        await DocumentEntrySeedShowcase.SeedDocuments(_context);
        _logger.LogInformation("Showcase data seed completed");
    }


}