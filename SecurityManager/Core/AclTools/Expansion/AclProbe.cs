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
      Group owningGroup = CreateOwningGroupEntry (aclProbe, role, ace);
      Tenant owningTenant = CreateOwningTenantEntry (aclProbe, user, ace);
      IList<AbstractRoleDefinition> abstractRoles = CreateAbstractRolesEntry (aclProbe, ace);

      aclProbe._securityToken = new SecurityToken (user, owningTenant, owningGroup, null, abstractRoles);
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
      switch (ace.TenantCondition)
      {
        case TenantCondition.OwningTenant:
          // Undecideable constraint: For ACE to match the SecurityToken.OwningTenant must be equal to the user's tenant.
          // Since this is undeciadeable, set the owning tenant so he will match, and record the constraint as an access condition.
          owningTenant = user.Tenant; 
          aclProbe.AccessConditions.IsOwningTenantRequired = true; 
          break;
        case TenantCondition.SpecificTenant:
          owningTenant = null; // Decideable constraint => no condition. Either Principal.Tenant matches or he does not.
          break;
        case TenantCondition.None:
          owningTenant = null; // No constraint => no condition. Will always match.
          break;
        default:
          throw new ArgumentException (String.Format ("ace.TenantSelection={0} is currently not supported by this method. Please extend method to handle the new TenantSelection state.", ace.TenantCondition));
      }
      return owningTenant;
    }

    private static Group CreateOwningGroupEntry (AclProbe aclProbe, Role role, AccessControlEntry ace)
    {
      Group owningGroup;
      switch (ace.GroupCondition)
      {
        case GroupCondition.OwningGroup:
          Assertion.IsNotNull (role.Group);
          owningGroup = role.Group;
          aclProbe.AccessConditions.OwningGroup = owningGroup;
          break;
        //case GroupCondition.BranchOfOwningGroup:
        //  Assertion.IsNotNull (role.Group);
        //  owningGroup = role.Group; // TODO: Change to "first parent groupo for which GroupType = ACE.GroupType"
        //  aclProbe.AccessConditions.HasOwningGroupCondition = true;
        //  break;
        case GroupCondition.SpecificGroup:
          owningGroup = null; // Decideable constraint => no condition. Either the Principal's groups contain the specifc group or not.
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          owningGroup = null; // Decideable constraint => no condition. Either one of the Principal's groups is of the specifc group type or not.
          break;
        case GroupCondition.None:
          owningGroup = null; // No constraint => no condition. Will always match.
          break;
        default:
          throw new ArgumentException (String.Format ("ace.GroupSelection={0} is currently not supported by this method. Please extend method to handle the new GroupSelection state.", ace.GroupCondition));
      }
      return owningGroup;
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