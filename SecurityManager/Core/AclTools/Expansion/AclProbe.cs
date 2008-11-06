using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <para>Contains a <see cref="SecurityToken"/> which can be used to query access rights through calling 
  /// <see cref="AccessControlList.GetAccessTypes(Domain.AccessControl.SecurityToken)"/>; the
  /// the permissions returned apply only if the <see cref="AclExpansionAccessConditions"/> of the <see cref="AclProbe"/> are satisfied.
  /// </para>
  /// <remarks><para>
  /// Instances can only be created through the <see cref="CreateAclProbe"/> factory method, which guarantees that 
  /// the <see cref="AclExpansionAccessConditions"/> correspond to the <see cref="SecurityToken"/>.
  /// </para></remarks>
  /// </summary>
  public class AclProbe
  {
    /// <summary>
    /// Factory method to create an <see cref="AclProbe"/> from the passed <see cref="User"/>, <see cref="Role"/> and <see cref="AccessControlEntry"/>.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <param name="ace"></param>
    /// <returns></returns>
    public static AclProbe CreateAclProbe (User user, Role role, AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("ace", ace);

      var aclProbe = new AclProbe ();
      IList<Group> owningGroups = CreateOwningGroupsEntry (aclProbe, role, ace);
      Tenant owningTenant = CreateOwningTenantEntry (aclProbe, user, ace);
      IList<AbstractRoleDefinition> abstractRoles = CreateAbstractRolesEntry (aclProbe, ace);

      aclProbe._securityToken = new SecurityToken (user, owningTenant, owningGroups, abstractRoles);
      //aclProbe._securityToken.SpecificAce = ace;
      return aclProbe;
    }

    private static IList<AbstractRoleDefinition> CreateAbstractRolesEntry (AclProbe aclProbe, AccessControlEntry ace)
    {
      IList<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();
      if (ace.SpecificAbstractRole != null)
      {
        var abstractRole = ace.SpecificAbstractRole;
        abstractRoles.Add (abstractRole);
        aclProbe.AccessConditions.AbstractRole = abstractRole;
      }
      return abstractRoles;
    }


    private static Tenant CreateOwningTenantEntry (AclProbe aclProbe, User user, AccessControlEntry ace)
    {
      Tenant owningTenant;
      switch (ace.TenantSelection)
      {
        case TenantSelection.OwningTenant:
          owningTenant = user.Tenant;
          aclProbe.AccessConditions.IsOwningTenantRequired = true;
          break;
        case TenantSelection.SpecificTenant:
        case TenantSelection.All:
          owningTenant = ace.SpecificTenant;
          break;
        default:
          throw new ArgumentException (String.Format ("ace.TenantSelection={0} is currently not supported by this method. Please extend method to handle the new TenantSelection state.", ace.TenantSelection));
      }
      return owningTenant;
    }

    private static IList<Group> CreateOwningGroupsEntry (AclProbe aclProbe, Role role, AccessControlEntry ace)
    {
      IList<Group> owningGroups = new List<Group> ();
      switch (ace.GroupSelection)
      {
        case GroupSelection.OwningGroup:
          Assertion.IsNotNull (role.Group);
          owningGroups.Add (role.Group);
          aclProbe.AccessConditions.IsOwningGroupRequired = true;
          break;
        case GroupSelection.All:
          // If the ACE contains no specific group, then the probe's owningGroups collection is empty.
          if (ace.SpecificGroup != null)
          {
            owningGroups.Add (ace.SpecificGroup);
          }
          break;
        default:
          throw new ArgumentException (String.Format ("ace.GroupSelection={0} is currently not supported by this method. Please extend method to handle the new GroupSelection state.", ace.GroupSelection));
      }
      return owningGroups;
    }

    // The SecurityToken that will be used in the call to AccessControlList.GetAccessTypes
    private SecurityToken _securityToken;
    // The access conditions that must be satisfied for the _securityToken to match; i.e. the permissions returned by
    // the call to AccessControlList.GetAccessTypes apply only if the access conditions are satisfied. 
    private readonly AclExpansionAccessConditions _accessConditions = new AclExpansionAccessConditions();

    // Create through factory only
    private AclProbe () {}


    public SecurityToken SecurityToken
    {
      get { return _securityToken; }
    }

    public AclExpansionAccessConditions AccessConditions
    {
      get { return _accessConditions; }
    }


  }
}