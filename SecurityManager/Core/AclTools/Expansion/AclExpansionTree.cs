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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

using Remotion.Development.UnitTesting.ObjectMother; 

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionTree : IToText
  {
    private readonly Func<AclExpansionEntry, string> _orderbyForSecurableClass;

    //List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>>>>
    List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>>
       _aclExpansionTree;

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion)
      : this (aclExpansion, (classEntry  => (classEntry.AccessControlList is StatelessAccessControlList) ? "" : classEntry.Class.DisplayName))
    {
    }

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


    private void CreateAclExpansionTree (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);

      var aclExpansionTree = (from entry in aclExpansion
                           orderby entry.User.DisplayName
                           group entry by entry.User
                           into grouping
                               select AclExpansionTreeNode.New (
                               grouping.Key,
                               CountRowsBelow (grouping),
                               (from roleEntry in grouping
                                orderby roleEntry.Role.Group.DisplayName , roleEntry.Role.Position.DisplayName
                                group roleEntry by roleEntry.Role
                                into roleGrouping
                                    select AclExpansionTreeNode.New (
                                    roleGrouping.Key,
                                    CountRowsBelow (roleGrouping),
                                    (from classEntry in roleGrouping
                                     orderby _orderbyForSecurableClass (classEntry)
                                     group classEntry by classEntry.Class
                                     into classGrouping
                                         select AclExpansionTreeNode.New (
                                         classGrouping.Key,
                                         CountRowsBelow (classGrouping),
                                         //classGrouping.ToList() // States, i.e. final AclExpansion detail level
                                         classGrouping.GroupBy (aee => aee, aee => aee,
        AclExpansionHtmlWriter.AclExpansionEntryIgnoreStateEqualityComparer).Select (x => AclExpansionTreeNode.New (x.Key, x.Count (), x.ToList ())).ToList()

                           )).ToList() )).ToList() )).ToList();


      _aclExpansionTree = aclExpansionTree;
    }

#if(false)
    private int CountRowsBelow<T> (IGrouping<T, AclExpansionEntry> grouping)
    {
      return grouping.Count ();
    }
#else
    private int CountRowsBelow<T> (IGrouping<T, AclExpansionEntry> grouping)
    {
      return grouping.Distinct (AclExpansionHtmlWriter.AclExpansionEntryIgnoreStateEqualityComparer).Count ();
    }
#endif


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.e (Tree);
    }
  }
}