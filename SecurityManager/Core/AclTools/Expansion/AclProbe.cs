using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
//using Remotion.Development.UnitTesting.ObjectMother;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclProbe
  {
    private SecurityToken _securityToken;
    private AclExpansionAccessConditions _aclExpansionAccessConditions = new AclExpansionAccessConditions();

    public static AclProbe CreateAclProbe (User user, Role role, AccessControlEntry ace)
    {
      var aclProbe = new AclProbe();
      //aclProbe._securityToken.User = user;
      Tenant owningTenant = null;
      IList<Group> owningGroups = new List<Group>();
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