using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace VTECHERP.EntityFrameworkCore;

public static class VTECHERPEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        VTECHERPGlobalFeatureConfigurator.Configure();
        VTECHERPModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            #region Note
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE VTECHERPModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:

                 ObjectExtensionManager.Instance
                     .MapEfCoreProperty<IdentityUser, string>(
                         "MyProperty",
                         (entityBuilder, propertyBuilder) =>
                         {
                             propertyBuilder.HasMaxLength(128);
                         }
                     );

             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */
            #endregion

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<IdentityUser, bool>(
                "IsVTech",
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasDefaultValue(true);
                });

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<IdentityUser, Guid>(
                "MainStoreId",
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.IsRequired();
                });

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<IdentityUser, string?>(
                "UserCode",
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasDefaultValueSql("FORMAT(NEXT VALUE FOR IdentityUserCode,'0000000000')");
                });

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<IdentityRole, string?>(
                "Description",
                (entityBuilder, propertyBuilder) =>
                {
                });

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<IdentityRole, Guid?>(
                "CreatorId",
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.IsRequired(false);
                });

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<Tenant, bool>(
                "IsVTech",
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.IsRequired();
                });
        });
    }
}
