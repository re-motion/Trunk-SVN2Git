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


    // TODO: Finish & test 
    private void CreateAclExpansionTreeSpike (List<AclExpansionEntry> aclExpansion)
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

      var aclExpansionTree = (from entry in aclExpansion
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


      //GetAclExpansionGrouping

      foreach (var userGroup in aclExpansionTree)
      {
        //WriteTableBody_ProcessUserGroup (userGroup);
      }
    }

    //private IList<T> GetStates (IGrouping<SecurableClassDefinition, AclExpansionEntry> classGrouping)
    //{

    //  classGrouping.
    //  //classGrouping.OfType<StatelessAccessControlList> ().
    //  var statefulStates = classGrouping.OfType<StatefulAccessControlList>().Select (
    //      x => x.StateCombinations.SelectMany (y => y.GetStates()).OrderBy (z => z.DisplayName));


    //  return statefulStates.ToList();
    //  //statefulStates.AddRange
    //}


    //public void WriteTableBody_ProcessUserGroup (List<AclExpansionEntry> aclExpansionList)
    //{
    //  var aclExpansionUserGrouping = GetAclExpansionGrouping (aclExpansionList, (x => x.User));

    //  foreach (var entry in aclExpansionUserGrouping)
    //  {
    //  }
    //}


    //private void WriteTableBody_ProcessUserGroup (IGrouping<User, AclExpansionEntry> userGroup)
    //{
    //  WriteTableDataWithRowCount (userGroup.Key.DisplayName, userGroup.Count ());

    //  var aclExpansionRoleGrouping = from aee in userGroup
    //                                 orderby aee.Role.Group.DisplayName, aee.Role.Position.DisplayName
    //                                 group aee by aee.Role;

    //  foreach (var roleGroup in aclExpansionRoleGrouping)
    //  {
    //    WriteTableBody_ProcessRoleGroup (roleGroup);
    //  }
    //}

    //private void WriteTableBody_ProcessRoleGroup (IGrouping<Role, AclExpansionEntry> roleGroup)
    //{
    //  WriteTableDataForRole (roleGroup.Key, roleGroup.Count ());

    //  var aclExpansionClassGrouping = from aee in roleGroup
    //                                  orderby aee.Class.DisplayName
    //                                  group aee by aee.Class;


    //  foreach (var classGroup in aclExpansionClassGrouping)
    //  {
    //    WriteTableBody_ProcessClassGroup (classGroup);
    //  }
    //}

    //private void WriteTableBody_ProcessClassGroup (IGrouping<SecurableClassDefinition, AclExpansionEntry> classGroup)
    //{
    //  if (classGroup.Key != null)
    //  {
    //    string className = Settings.ShortenNames ? classGroup.Key.ShortName () : classGroup.Key.DisplayName;
    //    WriteTableDataWithRowCount (className, classGroup.Count ());
    //  }
    //  else
    //  {
    //    WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classGroup.Count ());
    //  }

    //  foreach (var aclExpansionEntry in classGroup)
    //  {
    //    WriteTableRowBeginIfNotInTableRow ();

    //    WriteTableDataForBodyStates (aclExpansionEntry);
    //    WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
    //    WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);

    //    WriteTableRowEnd ();
    //  }
    //}   


    public void BuildUserGrouping ()
    {
      //var aclExpansionRoleGrouping = GetAclExpansionGrouping (userGroup, (x => x.Role));
    }

    //public IEnumerable<AclExpansionTreeNode<T, AclExpansionEntry>> GetAclExpansionGrouping<T, TIn> (
    //  AclExpansionTreeNode<TIn, AclExpansionEntry> aeeGroup,
    //  Func<AclExpansionEntry, T> groupingKeyFunc)
    //{
    //  return from aee in aeeGroup.Children
    //         group aee by groupingKeyFunc (aee)
    //           into groupEntries
    //           select Remotion.Development.UnitTesting.ObjectMother.AclExpansionTreeNode.New (groupEntries.Key, groupEntries.ToList(), groupEntries.Count());
    //}


    //public IEnumerable<AclExpansionTreeNode<User, AclExpansionEntry>> GetAclExpansionGrouping (IEnumerable<AclExpansionEntry> aclExpansion,
    //  Func<AclExpansionEntry, User> groupingKeyFunc)
    //{
    //  return from aee in aclExpansion
    //         group aee by groupingKeyFunc (aee)
    //           into groupEntries
    //           //select Remotion.Development.UnitTesting.ObjectMother.AclExpansionTreeNode.New (groupEntries);
    //           select Remotion.Development.UnitTesting.ObjectMother.AclExpansionTreeNode.New (groupEntries.Key, groupEntries.ToList(), groupEntries.Count());
    //}


    //public AclExpansionTreeNode<T1,T2> GetAclExpansionGrouping<T, TIn, T1, T2> (
    //  IGrouping<TIn, AclExpansionEntry> aeeGroup,
    //  Func<AclExpansionEntry, T> groupingKeyFunc)
    //{
    //  return from aee in aeeGroup
    //         group aee by groupingKeyFunc (aee)
    //           into groupEntries
    //           select Remotion.Development.UnitTesting.ObjectMother.AclExpansionTreeNode.New (groupEntries.Key, 
    //           groupEntries.ToList (), groupEntries.Count ());
    //}

  }
}