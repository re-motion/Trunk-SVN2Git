using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

//using Remotion.Development.UnitTesting.ObjectMother;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclProbe
  {
    private SecurityToken _securityToken;
    private AclExpansionAccessConditions _aclExpansionAccessConditions = new AclExpansionAccessConditions();

    
    public SecurityToken SecurityToken
    {
      get { return _securityToken; }
    }

    public AclExpansionAccessConditions AccessConditions
    {
      get { return _aclExpansionAccessConditions; }
    }


    public static AclProbe CreateAclProbe (User user, Role role, AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("ace", ace);

      IList<Group> owningGroups = CreateOwningGroupsEntry(role, ace);
      Tenant owningTenant = CreateOwningTenantEntry(user, ace);
      IList<AbstractRoleDefinition> abstractRoles = CreatAbstractRolesEntry(ace);

      var aclProbe = new AclProbe ();
      aclProbe._securityToken = new SecurityToken (user, owningTenant, owningGroups, abstractRoles);
      return aclProbe;
    }

    private static IList<AbstractRoleDefinition> CreatAbstractRolesEntry (AccessControlEntry ace)
    {
      IList<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition>();
      if (ace.SpecificAbstractRole != null)
      {
        abstractRoles.Add (ace.SpecificAbstractRole);
      }
      return abstractRoles;
    }


    private static Tenant CreateOwningTenantEntry (User user, AccessControlEntry ace)
    {
      Tenant owningTenant = null;
      switch (ace.TenantSelection)
      {
        case TenantSelection.OwningTenant:
          owningTenant = user.Tenant;
          break;
        case TenantSelection.SpecificTenant: 
        case TenantSelection.All:
          owningTenant = ace.SpecificTenant;
          break;
        default:
          throw new NotSupportedException (String.Format ("ace.TenantSelection={0} is currently not supported by this method. Please extend method to handle the new TenantSelection state.", ace.TenantSelection));
      }
      return owningTenant;
    }

    private static IList<Group> CreateOwningGroupsEntry (Role role, AccessControlEntry ace)
    {
      IList<Group> owningGroups = new List<Group>();
      switch (ace.GroupSelection)
      {
        case GroupSelection.OwningGroup:
          owningGroups.Add (role.Group); 
          break;
        case GroupSelection.All:
          owningGroups.Add (ace.SpecificGroup); 
          break;
        default:
          throw new NotSupportedException (String.Format("ace.GroupSelection={0} is currently not supported by this method. Please extend method to handle the new GroupSelection state.",ace.GroupSelection));
      }
      return owningGroups;
    }


  }
}