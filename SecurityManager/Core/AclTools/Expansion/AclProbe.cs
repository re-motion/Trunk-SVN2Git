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

    public static AclProbe CreateAclProbe (User user, Role role, AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("ace", ace);
      var aclProbe = new AclProbe();
      //aclProbe._securityToken.User = user;



      //Tenant owningTenant = (ace.TenantSelection == TenantSelection.OwningTenant) ? user.Tenant : ace.SpecificTenant;

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



      IList<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition>();

      aclProbe._securityToken = new SecurityToken(user,owningTenant,owningGroups,abstractRoles);
      return aclProbe;
    }

    public SecurityToken SecurityToken
    {
      get { return _securityToken; }
    }

    public AclExpansionAccessConditions AccessConditions
    {
      get { return _aclExpansionAccessConditions; }
    }
  }
}