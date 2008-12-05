// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
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
    /// <returns></returns>
    public static AclProbe CreateAclProbe (User user, Role role, AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("ace", ace);

      var aclProbe = new AclProbe ();
      User owningUser = CreateOwningUserEntry (aclProbe, user, ace);
      Group owningGroup = CreateOwningGroupEntry (aclProbe, role, ace);
      Tenant owningTenant = CreateOwningTenantEntry (aclProbe, user, ace);
      IList<AbstractRoleDefinition> abstractRoles = CreateAbstractRolesEntry (aclProbe, ace);

      //aclProbe._securityToken = new SecurityToken (user, owningTenant, owningGroup, null, abstractRoles);
      aclProbe._securityToken = new SecurityToken (user, owningTenant, owningGroup, owningUser, abstractRoles);
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

    private static User CreateOwningUserEntry (AclProbe aclProbe, User user, AccessControlEntry ace)
    {
      User owningUser;
      switch (ace.UserCondition)
      {
        case UserCondition.Owner:
          // Undecideable constraint: For ACE to match the SecurityToken.OwningUser must be equal to the user's user.
          // Since this is undeciadeable, set the owning user so he will match, and record the constraint as an access condition.
          owningUser = user;
          aclProbe.AccessConditions.IsOwningUserRequired = true;
          break;
        case UserCondition.SpecificUser:
          owningUser = null; // Decideable constraint => no condition. Either Principal matches or he does not.
          break;
        case UserCondition.SpecificPosition:
          owningUser = null; // Decideable constraint => no condition. Either Principal's position matches or it does not.
          break;
        case UserCondition.None:
          owningUser = null; // No constraint => no condition (will always match).
          break;
        default:
          throw new ArgumentException (String.Format ("ace.UserSelection={0} is currently not supported by this method. Please extend method to handle the new UserSelection state.", ace.UserCondition));
      }
      return owningUser;
    }


    private static Tenant CreateOwningTenantEntry (AclProbe aclProbe, User user, AccessControlEntry ace)
    {
      Tenant owningTenant;
      switch (ace.TenantCondition)
      {
        case TenantCondition.OwningTenant:
          // Undecideable constraint: For ACE to match the SecurityToken.OwningTenant must be equal to the user's tenant.
          // Since this is undecideable, set the owning tenant so he will match, and record the constraint as an access condition,
          // keeping in mind the TenantHierarchyCondition.

          // Owning tenant will be set to the user's tenant, which should never be empty. 
          Assertion.IsNotNull (user.Tenant);
          // TenantHierarchyCondition should always contain the flag for "this tenant"; 
          // if this condition is violated, using owningTenant = user.Tenant will no longer work, since it will not match.
          Assertion.IsTrue ((ace.TenantHierarchyCondition & TenantHierarchyCondition.This) != 0);
          owningTenant = user.Tenant;
          aclProbe.AccessConditions.OwningTenant = owningTenant;
          aclProbe.AccessConditions.TenantHierarchyCondition = ace.TenantHierarchyCondition;
          break;
        case TenantCondition.SpecificTenant:
          owningTenant = null; // Decideable constraint => no condition. Either Principal.Tenant matches or he does not.
          break;
        case TenantCondition.None:
          owningTenant = null; // No constraint => no condition (will always match).
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
          // Owning group will be set to the role group, which should never be empty.
          Assertion.IsNotNull (role.Group);
          // GroupHierarchyCondition should always contain the flag for "this group";
          // if this condition is violated, using owningGroup = role.Group will no longer work, since it will not match.
          Assertion.IsTrue ((ace.GroupHierarchyCondition & GroupHierarchyCondition.This) != 0); 
          owningGroup = role.Group;
          aclProbe.AccessConditions.OwningGroup = owningGroup;
          aclProbe.AccessConditions.GroupHierarchyCondition = ace.GroupHierarchyCondition;
          break;
        case GroupCondition.BranchOfOwningGroup:
          Assertion.IsNotNull (role.Group);
          owningGroup = FindFirstGroupInThisAndParentHierarchyWhichHasGroupType (role.Group, ace.SpecificGroupType);
          aclProbe.AccessConditions.OwningGroup = owningGroup;
          aclProbe.AccessConditions.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
          break;
        case GroupCondition.SpecificGroup:
          owningGroup = null; // Decideable constraint => no condition. Either the Principal's groups contain the specifc group or not.
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          owningGroup = null; // Decideable constraint => no condition. Either one of the Principal's groups is of the specifc group type or not.
          break;
        case GroupCondition.None:
          owningGroup = null; // No constraint => no condition (will always match).
          break;
        default:
          throw new ArgumentException (String.Format ("ace.GroupSelection={0} is currently not supported by this method. Please extend method to handle the new GroupSelection state.", ace.GroupCondition));
      }
      return owningGroup;
    }

    private static Group FindFirstGroupInThisAndParentHierarchyWhichHasGroupType (Group group, GroupType groupType)
    {
      var thisAndParents = GetThisAndParents(group);
      //To.ConsoleLine.e (() => thisAndParents);
      Group matchingGroup = thisAndParents.Where (g => g.GroupType == groupType).FirstOrDefault ();
      //To.ConsoleLine.e (() => matchingGroup).nl ();
      return matchingGroup;
    }

    // TODO: Remove code duplication as soon as SecurityTokenMatcher.GetThisAndParents is moved to generic Linq extension method.
    private static IEnumerable<Group> GetThisAndParents (Group group)
    {
      for (var current = group; current != null; current = current.Parent)
        yield return current;
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