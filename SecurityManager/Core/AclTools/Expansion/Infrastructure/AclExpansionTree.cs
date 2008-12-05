/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionTree : IToTextConvertible
  {
    private readonly Func<AclExpansionEntry, string> _orderbyForSecurableClass;
 
    // IEqualityComparer which ignores differences in states (AclExpansionEntry.StateCombinations) to
    // group AclExpansionEntry|s together which only differ in state.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryIgnoreStateEqualityComparer =
        new CompoundValueEqualityComparer<AclExpansionEntry> (a => new object[] {
            a.Class, a.Role, a.User,
            a.AccessConditions.AbstractRole,
            a.AccessConditions.GroupHierarchyCondition,
            a.AccessConditions.IsOwningUserRequired,
            a.AccessConditions.OwningGroup,
            a.AccessConditions.OwningTenant,
            a.AccessConditions.TenantHierarchyCondition,
            EnumerableEqualsWrapper.New (a.AllowedAccessTypes),
            EnumerableEqualsWrapper.New (a.DeniedAccessTypes) 
          }
        );

    private readonly List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, 
      AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>> _aclExpansionTree;

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion)
        : this (aclExpansion, (classEntry  => (classEntry.AccessControlList is StatelessAccessControlList) ? "" : classEntry.Class.DisplayName))
    {
    }

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion, Func<AclExpansionEntry, string> orderbyForSecurableClass)
    {
      _orderbyForSecurableClass = orderbyForSecurableClass;
      _aclExpansionTree = CreateAclExpansionTree (aclExpansion);
    }

    public List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, 
      AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>> Tree
    {
      get { return _aclExpansionTree; }
    }


    public static CompoundValueEqualityComparer<AclExpansionEntry> AclExpansionEntryIgnoreStateEqualityComparer
    {
      get { return _aclExpansionEntryIgnoreStateEqualityComparer; }
    }

    private List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition,
      AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>> 
      CreateAclExpansionTree (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);

      var aclExpansionTree = (aclExpansion.OrderBy (entry => entry.User.DisplayName).GroupBy (entry => entry.User).Select (
          grouping => AclExpansionTreeNode.New (grouping.Key, CountRowsBelow (grouping), RoleGrouping(grouping).ToList()))).ToList();

      return aclExpansionTree;
    }

    private IEnumerable<AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>> RoleGrouping (IGrouping<User, AclExpansionEntry> grouping)
    {
      return (grouping.OrderBy (roleEntry => roleEntry.Role.Group.DisplayName).ThenBy (roleEntry => roleEntry.Role.Position.DisplayName).
          GroupBy (roleEntry => roleEntry.Role).Select (
          roleGrouping => AclExpansionTreeNode.New (roleGrouping.Key, CountRowsBelow (roleGrouping), ClassGrouping(roleGrouping).ToList())));
    }

    private IEnumerable<AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>> ClassGrouping (IGrouping<Role, AclExpansionEntry> roleGrouping)
    {
      return (roleGrouping.OrderBy (classEntry => _orderbyForSecurableClass (classEntry)).GroupBy (
          classEntry => classEntry.Class).Select (
          classGrouping => AclExpansionTreeNode.New (classGrouping.Key, CountRowsBelow (classGrouping), StateGrouping (classGrouping).ToList())));
    }

    private List<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> StateGrouping (IGrouping<SecurableClassDefinition, AclExpansionEntry> classGrouping)
    {
      return classGrouping.GroupBy (
          aee => aee, aee => aee, AclExpansionEntryIgnoreStateEqualityComparer).
          Select (x => AclExpansionTreeNode.New (x.Key, x.Count(), x.ToList())).ToList();
    }



    private int CountRowsBelow<T> (IGrouping<T, AclExpansionEntry> grouping)
    {
      return grouping.Distinct (AclExpansionEntryIgnoreStateEqualityComparer).Count ();
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.e (Tree);
    }
  }
}