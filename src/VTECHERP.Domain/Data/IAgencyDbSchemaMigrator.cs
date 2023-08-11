using System.Threading.Tasks;

namespace VTECHERP.Data;

public interface IAgencyDbSchemaMigrator
{
    Task MigrateAsync();
}
