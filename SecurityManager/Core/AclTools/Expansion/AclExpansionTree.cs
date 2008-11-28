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
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

using Remotion.Development.UnitTesting.ObjectMother; 

namespace Remotion.SecurityManager.AclTools.Expansion
{
  // TODO AE: Remove commented code. (Do not commit.)
  public class AclExpansionTree : IToText
  {
    private readonly Func<AclExpansionEntry, string> _orderbyForSecurableClass;
 
    // IEqualityComparer which ignores differences in states (AclExpansionEntry.StateCombinations) to
    // group AclExpansionEntry|s together which only differ in state.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryIgnoreStateEqualityComparer =
      new CompoundValueEqualityComparer<AclExpansionEntry> (a => new object[] {
          //a.AccessControlList, a.Class, a.Role, a.User,
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

    // TODO AE: Add "private".
    //List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>>>>
    List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>>
       _aclExpansionTree;

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion)
      : this (aclExpansion, (classEntry  => (classEntry.AccessControlList is StatelessAccessControlList) ? "" : classEntry.Class.DisplayName))
    {
    }

    // TODO AE: orderbyForSecurableClass seems to always be the same lamda. Inline it to reduce complexity and remove ctor parameter.
    public AclExpansionTree (List<AclExpansionEntry> aclExpansion, Func<AclExpansionEntry, string> orderbyForSecurableClass)
    {
      _orderbyForSecurableClass = orderbyForSecurableClass;
      CreateAclExpansionTree (aclExpansion);
    }

    //public List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>>>> Tree
    public List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>> Tree
    {
      get { return _aclExpansionTree; }
    }


    public static CompoundValueEqualityComparer<AclExpansionEntry> AclExpansionEntryIgnoreStateEqualityComparer
    {
      get { return _aclExpansionEntryIgnoreStateEqualityComparer; }
    }

    // TODO AE: Make method return the tree rather than setting a member; then make member immutable.
    private void CreateAclExpansionTree (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);



      //var aclExpansionTree = (from entry in aclExpansion
      //                        orderby entry.User.DisplayName
      //                        group entry by entry.User
      //                          into grouping
      //                          select AclExpansionTreeNode.New (
      //                          grouping.Key,
      //                          CountRowsBelow (grouping),
      //                          (from roleEntry in grouping
      //                           orderby roleEntry.Role.Group.DisplayName, roleEntry.Role.Position.DisplayName
      //                           group roleEntry by roleEntry.Role
      //                             into roleGrouping
      //                             select AclExpansionTreeNode.New (
      //                             roleGrouping.Key,
      //                             CountRowsBelow (roleGrouping),
      //                             (from classEntry in roleGrouping
      //                              orderby _orderbyForSecurableClass (classEntry)
      //                              group classEntry by classEntry.Class
      //                                into classGrouping
      //                                select AclExpansionTreeNode.New (
      //                                classGrouping.Key,
      //                                CountRowsBelow (classGrouping),
      //                                  //classGrouping.ToList() // States, i.e. final AclExpansion detail level
      //                                classGrouping.GroupBy (aee => aee, aee => aee, AclExpansionEntryIgnoreStateEqualityComparer
      //                                ).Select (x => AclExpansionTreeNode.New (x.Key, x.Count (), x.ToList ())).ToList ()

      //                  )).ToList ())).ToList ())).ToList ();

      var aclExpansionTree = (aclExpansion.OrderBy (entry => entry.User.DisplayName).GroupBy (entry => entry.User).Select (
          grouping => AclExpansionTreeNode.New (grouping.Key, CountRowsBelow (grouping), RoleGrouping(grouping).ToList()))).ToList();


      _aclExpansionTree = aclExpansionTree;
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
          //classGrouping => AclExpansionTreeNode.New (classGrouping.Key, CountRowsBelow (classGrouping), StateGrouping(classGrouping))));
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
