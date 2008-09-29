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
    private readonly AclExpansionAccessConditions _accessConditions = new AclExpansionAccessConditions();

    
    public SecurityToken SecurityToken
    {
      get { return _securityToken; }
    }

    public AclExpansionAccessConditions AccessConditions
    {
      get { return _accessConditions; }
    }


    public static AclProbe CreateAclProbe (User user, Role role, AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("ace", ace);

      var aclProbe = new AclProbe ();
      IList<Group> owningGroups = CreateOwningGroupsEntry (aclProbe, role, ace);
      Tenant owningTenant = CreateOwningTenantEntry (aclProbe, user, ace);
      IList<AbstractRoleDefinition> abstractRoles = CreatAbstractRolesEntry (aclProbe, ace);

      aclProbe._securityToken = new SecurityToken (user, owningTenant, owningGroups, abstractRoles);
      return aclProbe;
    }

    private static IList<AbstractRoleDefinition> CreatAbstractRolesEntry (AclProbe aclProbe, AccessControlEntry ace)
    {
      IList<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition>();
      if (ace.SpecificAbstractRole != null)
      {
        var abstractRole = ace.SpecificAbstractRole;
        abstractRoles.Add (abstractRole);
        aclProbe.AccessConditions.AbstractRole = abstractRole;
        aclProbe.AccessConditions.OnlyIfAbstractRoleMatches = true;
      }
      return abstractRoles;
    }


    private static Tenant CreateOwningTenantEntry (AclProbe aclProbe, User user, AccessControlEntry ace)
    {
      Tenant owningTenant = null;
      switch (ace.TenantSelection)
      {
        case TenantSelection.OwningTenant:
          owningTenant = user.Tenant;
          aclProbe.AccessConditions.OnlyIfTenantIsOwner = true;
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

    private static IList<Group> CreateOwningGroupsEntry (AclProbe aclProbe, Role role, AccessControlEntry ace)
    {
      IList<Group> owningGroups = new List<Group>();
      switch (ace.GroupSelection)
      {
        case GroupSelection.OwningGroup:
          owningGroups.Add (role.Group);
          aclProbe.AccessConditions.OnlyIfGroupIsOwner = true;
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