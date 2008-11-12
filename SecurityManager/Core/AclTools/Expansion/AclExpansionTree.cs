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
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

using Remotion.Development.UnitTesting.ObjectMother; 

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionTree
  {
    List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>>>>
       _aclExpansionTree;

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion)
    {
      CreateAclExpansionTree (aclExpansion);
    }

    // TODO: Finish & test 
    public List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>>>> Tree
    {
      get { return _aclExpansionTree; }
    }


    private void CreateAclExpansionTree (List<AclExpansionEntry> aclExpansion)
    {

      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);

      //var aclExpansionUserGrouping = from aee in aclExpansion
      //                               orderby aee.User.DisplayName
      //                               group aee by aee.User;


      //var aclExpansionUserGrouping = from entry in aclExpansion
      //                               orderby entry.User.DisplayName
      //                               group entry by entry.User
      //                               into grouping
      //                               select grouping;

      //var aclExpansionUserGrouping = from entry in aclExpansion
      //                               orderby entry.User.DisplayName
      //                               group entry by entry.User
      //                               into grouping
      //                               select AclExpansionTreeNode.New (grouping.Key, grouping.ToList (), grouping.Count ());

      //var aclExpansionUserGrouping = (from entry in aclExpansion
      //                               orderby entry.User.DisplayName
      //                               group entry by entry.User
      //                                 into grouping
      //                                 select AclExpansionTreeNode.New (grouping, grouping.ToList())).ToList();


      //var aclExpansionUserGrouping = (from entry in aclExpansion
      //                                orderby entry.User.DisplayName
      //                                group entry by entry.User
      //                                  into grouping
      //                                  select AclExpansionTreeNode.New (grouping,
      //                                  (from roleEntry in grouping
      //                                   orderby roleEntry.Role.DisplayName
      //                                   group roleEntry by roleEntry.Role
      //                                   ).ToList ()
      //                                  )).ToList ();


      //var aclExpansionUserGrouping = (from entry in aclExpansion
      //                                orderby entry.User.DisplayName
      //                                group entry by entry.User
      //                                  into grouping
      //                                  select AclExpansionTreeNode.New (grouping.Key,grouping.Count(),
      //                                  (from roleEntry in grouping
      //                                   orderby roleEntry.Role.DisplayName
      //                                   group roleEntry by roleEntry.Role
      //                                   into roleGrouping
      //                                   select AclExpansionTreeNode.New (roleGrouping.Key,roleGrouping.Count(),
                                         
      //                                    roleGrouping.ToList()

      //                                   )

      //                                   ).ToList ()
      //                                  )).ToList ();


      //var aclExpansionUserGrouping = (from entry in aclExpansion
      //                                orderby entry.User.DisplayName
      //                                group entry by entry.User
      //                                  into grouping
      //                                  select AclExpansionTreeNode.New (grouping.Key, grouping.Count (),
      //                                  (from roleEntry in grouping
      //                                   orderby roleEntry.Role.DisplayName
      //                                   group roleEntry by roleEntry.Role
      //                                     into roleGrouping
      //                                     select AclExpansionTreeNode.New (roleGrouping.Key, roleGrouping.Count (),

      //                                      (from classEntry in roleGrouping
      //                                       orderby classEntry.Class.DisplayName
      //                                       group classEntry by classEntry.Class
      //                                         into classGrouping
      //                                         select AclExpansionTreeNode.New (classGrouping.Key, classGrouping.Count (),

      //                                          classGrouping.ToList ()

      //                                         )

      //                                   ).ToList ()

      //                                     )

      //                                   ).ToList ()
      //                                  )).ToList ();

      _aclExpansionTree = (from entry in aclExpansion
                            orderby entry.User.DisplayName
                            group entry by entry.User
                              into grouping
                              select AclExpansionTreeNode.New (grouping.Key, grouping.Count (),
                              (from roleEntry in grouping
                               orderby roleEntry.Role.DisplayName
                               group roleEntry by roleEntry.Role
                                 into roleGrouping
                                 select AclExpansionTreeNode.New (roleGrouping.Key, roleGrouping.Count (),

                                  (from classEntry in roleGrouping
                                   orderby classEntry.Class.DisplayName // TODO: Order by stateless/stateful first, then by DisplayName (map stateless to "" name ?)
                                   group classEntry by classEntry.Class
                                     into classGrouping
                                     select AclExpansionTreeNode.New (classGrouping.Key, classGrouping.Count (),
                                      classGrouping.ToList() // States, i.e. final AclExpansion detail level

                                      // TODO: Move StateCombinations flattening to its own class for testing
                                       // OR TODO: Create AclExpansionTreeNode for each state which contains IList<StateCombination>

                                     //(from stateEntry in classGrouping
                                     // select stateEntry.StateCombinations.SelectMany (x => x.GetStates ()).OrderBy (x => x.DisplayName)).ToList ()

                                     //classGrouping.Select (x => x.StateCombinations.SelectMany (y => y.GetStates ()).OrderBy (z => z.DisplayName)).ToList ()

                                     //classGrouping.OfType<StatelessAccessControlList> ().
                                     //classGrouping.OfType<StatefulAccessControlList> ().Select (
                                     // x => x.StateCombinations.SelectMany (y => y.GetStates ()).OrderBy (z => z.DisplayName)).ToList ()

                                     //classGrouping.OfType<StatefulAccessControlList> ().Select (
                                     // x => x.StateCombinations.SelectMany (y => y.GetStates ()).OrderBy (z => z.DisplayName)).Select(a => a.)

                                      //GetStates(classGrouping)

                                     )

                               ).ToList ()

                                 )

                               ).ToList ()
                              )).ToList ();

    }



  }
}