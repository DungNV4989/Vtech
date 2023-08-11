using VTECHERP.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Identity;

namespace VTECHERP.Permissions;

public class VTECHERPPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var groups = VTECHERPPermissions.GetAllGroups();
        foreach(var group in groups)
        {
            var permissionGroup = context.AddGroup(group.GroupCode);
            var groupPermissions = VTECHERPPermissions.GetGroupPermission(group.ModuleName,group.GroupName);
            foreach(var permission in groupPermissions)
            {
                permissionGroup.AddPermission(
                    permission.PermissionCode,
                    multiTenancySide: MultiTenancySides.Tenant);
            }
        }
        //var identityGroup = context.GetGroup("AbpIdentity.Users");
        //identityGroup.AddPermission("List");
        //Define your own permissions here. Example:
        //myGroup.AddPermission(VTECHERPPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<VTECHERPResource>(name);
    }
}
