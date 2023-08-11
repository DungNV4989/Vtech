using System.Threading.Tasks;

namespace VTECHERP.Data;

public interface IIdentityDbSchemaMigrator
{
    Task MigrateAsync();
}
