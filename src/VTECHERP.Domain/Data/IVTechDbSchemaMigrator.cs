using System.Threading.Tasks;

namespace VTECHERP.Data;

public interface IVTechDbSchemaMigrator
{
    Task MigrateAsync();
}
