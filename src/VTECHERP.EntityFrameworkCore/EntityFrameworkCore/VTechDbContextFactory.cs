using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VTECHERP.EntityFrameworkCore;

public class VTechDbContextFactory : IDesignTimeDbContextFactory<VTechDbContext>
{
    public VTechDbContext CreateDbContext(string[] args)
    {
        VTECHERPEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<VTechDbContext>()
            .UseSqlServer(configuration.GetConnectionString("VTech"));

        return new VTechDbContext(builder.Options);
    }
   
    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../VTECHERP.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
