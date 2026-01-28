using JdJobs.Data;
using JdJobs.Data.Entities;

namespace DataSource.Api.Tests.Infrastructure;

/// <summary>
/// Minimal seed data for JdJobs database (for DataSource view tests)
/// </summary>
public static class JdJobsTestSeeder
{
    public static void SeedMinimalData(JdJobsDbContext context)
    {
        if (context.BenefitsDict.Any()) return;

        // Benefits
        context.BenefitsDict.AddRange(
            new BenefitsDict { Id = 1, NamePL = "Karta sportowa", NameEN = "Sport card", Category = "Sport", IsActive = true },
            new BenefitsDict { Id = 2, NamePL = "Prywatna opieka medyczna", NameEN = "Private healthcare", Category = "Zdrowie", IsActive = true }
        );

        // Tools
        context.ToolsDict.AddRange(
            new ToolsDict { Id = 1, NamePL = "Microsoft Office", NameEN = "Microsoft Office", Category = "Office", IsActive = true },
            new ToolsDict { Id = 2, NamePL = "Visual Studio", NameEN = "Visual Studio", Category = "Development", IsActive = true }
        );

        // Traits (no Category field)
        context.TraitsDict.AddRange(
            new TraitsDict { Id = 1, NamePL = "Komunikatywność", NameEN = "Communication skills", IsActive = true },
            new TraitsDict { Id = 2, NamePL = "Praca w zespole", NameEN = "Teamwork", IsActive = true }
        );

        // Certificates
        context.CertificatesDict.AddRange(
            new CertificatesDict { Id = 1, NamePL = "AWS Certified", NameEN = "AWS Certified", Category = "Cloud", IsActive = true },
            new CertificatesDict { Id = 2, NamePL = "PMP", NameEN = "PMP", Category = "Management", IsActive = true }
        );

        // Positions
        context.PositionDicts.AddRange(
            new PositionDict { Id = 1, NamePL = "Developer", NameEN = "Developer", IsActive = true },
            new PositionDict { Id = 2, NamePL = "Project Manager", NameEN = "Project Manager", IsActive = true }
        );

        context.SaveChanges();
    }
}
