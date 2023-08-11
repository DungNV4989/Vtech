using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VTECHERP.EntityFrameworkCore;

public class AgencyDbContextFactory : IDesignTimeDbContextFactory<AgencyDbContext>
{
    public AgencyDbContext CreateDbContext(string[] args)
    {
        VTECHERPEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<AgencyDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Agency"));

        return new AgencyDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../VTECHERP.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
