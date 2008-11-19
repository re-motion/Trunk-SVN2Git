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
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Rhino.Mocks;

using List = Remotion.Development.UnitTesting.ObjectMother.List;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderTest : AclToolsTestBase
  {
    private readonly List<AccessControlList> _aclList = new List<AccessControlList>();
    private readonly List<User> _userList = new List<User> ();

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      TestHelper.CreateAceWithSpecificTenant (Tenant);
      _aclList.Add (TestHelper.CreateStatefulAcl());
    }


    [Test]
    [Explicit]
    public void AccessControlList_GetAccessTypes ()
    {
      var user = User;
      List<AccessControlList> aclList = new List<AccessControlList> ();

      AccessControlEntry ace = AccessControlEntry.NewObject ();
      //AccessTypeDefinition readAccessType = TestHelper.CreateReadAccessTypeAndSetWithValueAtAce (ace, true);
      //AccessTypeDefinition writeAccessType = TestHelper.CreateWriteAccessTypeAndSetWithValueAtAce (ace, true);
      //AccessTypeDefinition deleteAccessType = TestHelper.CreateDeleteAccessTypeAndSetWithValueAtAce (ace, true);

      AccessControlList acl = TestHelper.CreateStatefulAcl (ace);
      aclList.Add (acl);

      SecurityToken securityToken = new SecurityToken (user, User.Tenant, null, null, new List<AbstractRoleDefinition> ());
      AccessInformation accessInformation = acl.GetAccessTypes (securityToken);
      To.ConsoleLine.sb ().e (accessInformation.AllowedAccessTypes.Length).e (() => accessInformation.AllowedAccessTypes).se ();
    }


    [Test]
    public void AccessControlList_GetAccessTypes2 ()
    {
      var user = User3;
      //var acl = TestHelper.CreateAcl (Ace3, Ace2, Ace);
      var acl = TestHelper.CreateStatefulAcl (Ace3);
      Assert.That (Ace3.Validate().IsValid);
      SecurityToken securityToken = new SecurityToken (user, user.Tenant, null, null, new List<AbstractRoleDefinition> ());
      AccessInformation accessInformation = acl.GetAccessTypes (securityToken);
      //To.ConsoleLine.s ("AccessControlList_GetAccessTypes2: ").sb ().e (() => accessTypeDefinitions).se ();

      Assert.That (accessInformation.AllowedAccessTypes, Is.EquivalentTo (new[] { ReadAccessType, WriteAccessType }));
    }

    [Test]
    public void AccessControlList_GetAccessTypes_AceWithPosition_GroupSelectionAll ()
    {
      var ace = TestHelper.CreateAceWithPosition (Position, GroupCondition.None);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);

      Assert.That (ace.Validate ().IsValid);
      
      var acl = TestHelper.CreateStatefulAcl (ace);
      SecurityToken securityToken = new SecurityToken (User, User.Tenant, null, null, new List<AbstractRoleDefinition> ());
      AccessInformation accessInformation = acl.GetAccessTypes (securityToken);
   
      //To.ConsoleLine.s ("AccessControlList_GetAccessTypes2: ").sb ().e (() => accessTypeDefinitions).se ();

      Assert.That (accessInformation.AllowedAccessTypes, Is.EquivalentTo (new[] { ReadAccessType, DeleteAccessType }));
    }

    [Test]
    public void AccessControlList_GetAccessTypes_AceWithPosition_GroupSelectionOwningGroup ()
    {
      var ace = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);

      Assert.That (ace.Validate ().IsValid);

      var acl = TestHelper.CreateStatefulAcl (ace);
      // We pass the Group used in the ace Position above in the owningGroups-list => ACE will match.
      SecurityToken securityToken = new SecurityToken (User, User.Tenant, Group, null, new List<AbstractRoleDefinition> ());
      AccessInformation accessInformation = acl.GetAccessTypes (securityToken);

      //To.ConsoleLine.s ("AccessControlList_GetAccessTypes2: ").sb ().e (() => accessTypeDefinitions).se ();

      Assert.That (accessInformation.AllowedAccessTypes, Is.EquivalentTo (new[] { ReadAccessType, DeleteAccessType }));
    }



    [Test]
    public void GetAclExpansionEntryList_AceWithPosition_GroupSelectionAll ()
    {
      List<AclExpansionEntry> aclExpansionEntryList = 
        GetAclExpansionEntryList_UserPositionGroupSelection(User,Position,GroupCondition.None, GroupHierarchyCondition.Undefined);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions();
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }

    [Test]
    public void GetAclExpansionEntryList_AceWithPosition_GroupSelectionOwningGroup ()
    {
      List<AclExpansionEntry> aclExpansionEntryList = 
        GetAclExpansionEntryList_UserPositionGroupSelection (User, Position, GroupCondition.OwningGroup, GroupHierarchyCondition.This);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions () { 
        OwningGroup = aclExpansionEntryList[0].Role.Group, //  GroupSelection.OwningGroup => group must be owner 
        GroupHierarchyCondition = GroupHierarchyCondition.This
      };
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }

    [Test]
    public void GetAclExpansionEntryList_UserList_AceList ()
    {
      var ace = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);
      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (User),
          List.New(TestHelper.CreateStatefulAcl(ace))  
        );

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions ()
      {
        //HasOwningGroupCondition = true, //  GroupSelection.OwningGroup => group must be owner
        OwningGroup = User.Roles[0].Group, //  GroupSelection.OwningGroup => group must be owner
        GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent
      };
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_MultipleAces ()
    {
      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      var aceAbstractRole = TestHelper.CreateAceWithAbstractRole ();
      AttachAccessTypeReadWriteDelete (aceAbstractRole, null, false, true);

      var aceGroupAll = TestHelper.CreateAceWithoutGroupCondition ();
      AttachAccessTypeReadWriteDelete (aceGroupAll, true, true, null);

      //To.ConsoleLine.e (() => aceGroupOwning);
      //To.ConsoleLine.e (() => aceAbstractRole);
      //To.ConsoleLine.e (() => aceGroupAll);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (User),
          List.New (TestHelper.CreateStatefulAcl (aceGroupOwning, aceAbstractRole, aceGroupAll))
        );

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType, DeleteAccessType }, 
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[0].Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });

      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { ReadAccessType, DeleteAccessType },
       new AclExpansionAccessConditions { AbstractRole = aceAbstractRole.SpecificAbstractRole });

      AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { ReadAccessType, WriteAccessType },
       new AclExpansionAccessConditions());
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_AnotherTenant ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup ("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceGroupSpecificTenant = TestHelper.CreateAceWithSpecificTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (aceGroupSpecificTenant, null, true, null);


      //To.ConsoleLine.e (() => aceGroupSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (otherTenantUser),
          List.New (TestHelper.CreateStatefulAcl (aceGroupSpecificTenant))
        );


      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { WriteAccessType },
        new AclExpansionAccessConditions());
    }


    [Test]
    public void SpecificTenantAndOwningTenantTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup ("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceGroupSpecificTenant = TestHelper.CreateAceWithSpecificTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (aceGroupSpecificTenant, null, true, null);

      var aceGroupOwningTenant = TestHelper.CreateAceWithOwningTenant ();
      AttachAccessTypeReadWriteDelete (aceGroupOwningTenant, null, null, true);

      //To.ConsoleLine.e (() => aceGroupSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (otherTenantUser),
          List.New (TestHelper.CreateStatefulAcl (aceGroupSpecificTenant, aceGroupOwningTenant))
        );


      //To.ConsoleLine.e (() => aclExpansionEntryList);

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (2));

      // Test of specific-tenant-ACE: gives write-access
      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions ());

      // Test of owning-tenant-ACE (specific-tenant-ACE matches also): gives write-access + delete-access with condition: tenant-must-own
      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType }, 
        new AclExpansionAccessConditions { IsOwningTenantRequired = true });
    }



    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_TwoDifferentTenants ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup ("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceSpecificTenantWithOtherTenant = TestHelper.CreateAceWithSpecificTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (aceSpecificTenantWithOtherTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      //To.ConsoleLine.e (() => otherTenantAceSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (otherTenantUser, User),
          //List.New (otherTenantUser),
          List.New (TestHelper.CreateStatefulAcl (aceSpecificTenantWithOtherTenant, aceGroupOwning))
        );


      //Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (2));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions ());

      //AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { ReadAccessType, WriteAccessType },
      //  new AclExpansionAccessConditions { IsOwningGroupRequired = true });

      //AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { ReadAccessType, DeleteAccessType },
      //  new AclExpansionAccessConditions { IsOwningGroupRequired = true });

      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { ReadAccessType, DeleteAccessType },
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[1].Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_MultipleAces ()
    {
      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant();
      AttachAccessTypeReadWriteDelete (aceOwningTenant, true, true, null);
      
      var acePosition = TestHelper.CreateAceWithPosition (Position2, GroupCondition.None);
      AttachAccessTypeReadWriteDelete (acePosition, true, null, true);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, false, null);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (User),
          List.New (TestHelper.CreateStatefulAcl (aceOwningTenant, acePosition, aceGroupOwning))
        );

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (2));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions { IsOwningTenantRequired = true });

      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { ReadAccessType },
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[1].Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });
     }



    [Test]
    public void GetAclExpansionEntryList_UserList_SeparateAcls ()
    {
      //var aceMatchAll = TestHelper.CreateAceWithGroupSelectionAll();
      //AttachAccessTypeReadWriteDelete (aceMatchAll, true, null, null);

      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant ();
      AttachAccessTypeReadWriteDelete (aceOwningTenant, true, true, null);

      var aceSpecificTenant = TestHelper.CreateAceWithSpecificTenant (Tenant);
      AttachAccessTypeReadWriteDelete (aceSpecificTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (User),
          //List.New (TestHelper.CreateAcl (aceMatchAll, aceSpecificTenant, aceGroupOwning))
          List.New (TestHelper.CreateStatefulAcl (aceOwningTenant), TestHelper.CreateStatefulAcl (aceSpecificTenant), TestHelper.CreateStatefulAcl (aceGroupOwning))
        );


      var aclExpansionEntryListEnumerator = aclExpansionEntryList.GetEnumerator ();

      aclExpansionEntryListEnumerator.MoveNext ();
      AssertAclExpansionEntry (aclExpansionEntryListEnumerator.Current, new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions { IsOwningTenantRequired = true });

      aclExpansionEntryListEnumerator.MoveNext ();
      AssertAclExpansionEntry (aclExpansionEntryListEnumerator.Current, new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions ());

      aclExpansionEntryListEnumerator.MoveNext ();
      AssertAclExpansionEntry (aclExpansionEntryListEnumerator.Current, new[] { ReadAccessType, DeleteAccessType },
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryListEnumerator.Current.Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });

      Assert.That (aclExpansionEntryListEnumerator.MoveNext (), Is.EqualTo (false));
    }



    [Test]
    public void GetAclExpansionEntryList_UserList_IUserRoleAclAceCombinations ()
    {
      var userRoleAclAceCombinationsMock = MockRepository.GenerateMock<IUserRoleAclAceCombinations>();
      //var myValues = new [] {  new UserRoleAclAceCombination(Role,Ace) };
      var myValues = List.New(new UserRoleAclAceCombination (Role, Ace) );     
      userRoleAclAceCombinationsMock.Expect (mock => mock.GetEnumerator ()).Return (myValues.GetEnumerator ());

      var aclExpander = new AclExpander (userRoleAclAceCombinationsMock);
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList ();
      //To.ConsoleLine.e (() => aclExpansionEntryList);
      userRoleAclAceCombinationsMock.VerifyAllExpectations ();
    }



    [Test]
    [Explicit]
    public void GetAclExpansionEntryList_ComplexExpansionTest ()
    { 
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User,User2,User3);

      var numberRoles = users.SelectMany (x => x.Roles).Count ();
      //To.ConsoleLine.e (() => numberRoles);
      
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl,Acl2);

      var numberAces = acls.SelectMany (x => x.AccessControlEntries).Count ();
      //To.ConsoleLine.e (() => numberAces); 

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      WriteAclExpansionAsHtmlSpikeToStreamWriter (aclExpansionEntryList, true);
    }



    [Test]
    public void NonMatchingAceTest_SpecificTenant ()
    {
      // Together with User or User2 gives non-matching ACEs
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var testAce = TestHelper.CreateAceWithSpecificTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (testAce, true, true, true);

      Assert.That (testAce.Validate ().IsValid);

      //To.ConsoleLine.s (ace.ToString ());

      var userList = List.New (User, User2);
      var aclList = List.New (TestHelper.CreateStatefulAcl (testAce));

      // ACE with specific otherTenant should not match any AclProbe|s
      AssertIsNotInMatchingAces(userList, aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList,aclList);

      //To.ConsoleLine.e (() => aclExpansionEntryList);

      // If ACE does not macth, the resulting aclExpansionEntryList must be empty.
      Assert.That (aclExpansionEntryList, Is.Empty);
    }

    [Test]
    public void CurrentRoleOnly_SpecificPostitonWithOwningGroupTest ()
    {
      var testAce = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (testAce, true, true, true);

      Assert.That (testAce.Validate ().IsValid);

      //To.ConsoleLine.s (ace.ToString ());

      var userList = List.New (User2);
      var aclList = List.New (TestHelper.CreateStatefulAcl (testAce));

      // ACE with specific position should not match any AclProbe|s
      //OutputAccessStatistics (userList, aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      // To.ConsoleLine.nl().e (() => aclExpansionEntryList); To.ConsoleLine.nl().e (User2.Roles);

      // Note: Without "current role only"-probing in AclExpander.GetAccessTypes, the role {"Anotha Group","Working Drone"}
      // would also give an aclExpansionEntryList entry.
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));


      var aclExpansion = aclExpansionEntryList[0];

      To.ConsoleLine.e (aclExpansion.AccessConditions.GroupHierarchyCondition);

      AssertAclExpansionEntry (aclExpansion, new[] { ReadAccessType, WriteAccessType, DeleteAccessType },
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[0].Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });

      Assert.That (aclExpansion.User, Is.EqualTo (User2));
      // Note: With "current role only"-probing in AclExpander.GetAccessTypes returns only access types for the role
      // {"Anotha Group","Supreme Being"} (and not also {"Anotha Group","Working Drone"}).
      Assert.That (aclExpansion.Role, Is.EqualTo (User2.Roles[2])); //
    }



    [Test]
    public void AbstractRoleAllContributingTest ()
    {
      var ace = TestHelper.CreateAceWithAbstractRole ();
      AttachAccessTypeReadWriteDelete (ace, true, true, true);

      Assert.That (ace.Validate ().IsValid);

      var userList = List.New (User, User2);
      var aclList = List.New (TestHelper.CreateStatefulAcl (ace));

      //OutputAccessStatistics (userList, aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      //To.ConsoleLine.nl ().e (() => aclExpansionEntryList);
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (User.Roles.Count + User2.Roles.Count));
    }


    [Test]
    public void AbstractRoleAllContributingDenyTest ()
    {
      var ace = TestHelper.CreateAceWithAbstractRole ();
      AttachAccessTypeReadWriteDelete (ace, true, true, true);
      Assert.That (ace.Validate ().IsValid);

      // In addition to AbstractRoleAllContributingTest, deny all rights again => 
      // there should be no resulting AclExpansionEntry|s.
      var aceDeny = TestHelper.CreateAceWithoutGroupCondition ();
      AttachAccessTypeReadWriteDelete (aceDeny, false, false, false);
      Assert.That (aceDeny.Validate ().IsValid);

      var userList = List.New (User, User2);
      var aclList = List.New (TestHelper.CreateStatefulAcl (ace, aceDeny));

      //OutputAccessStatistics (userList, aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      //To.ConsoleLine.nl ().e (() => aclExpansionEntryList);
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (0));
    }


    [Test]
    public void AclExpansionEntryDeniedRightsTest ()
    {
      var ace = TestHelper.CreateAceWithoutGroupCondition();
      AttachAccessTypeReadWriteDelete (ace, true, true, true);
      Assert.That (ace.Validate ().IsValid);

      // Deny read and delete rights
      var aceDeny = TestHelper.CreateAceWithoutGroupCondition ();
      AttachAccessTypeReadWriteDelete (aceDeny, false, true, false);
      Assert.That (aceDeny.Validate ().IsValid);

      var userList = List.New (User); 
      var aclList = List.New (TestHelper.CreateStatefulAcl (ace, aceDeny));

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      //To.ConsoleLine.nl ().e (() => aclExpansionEntryList);

      foreach (AclExpansionEntry aee in aclExpansionEntryList)
      {
        Assert.That (aee.AllowedAccessTypes, Is.EquivalentTo (new[] { WriteAccessType }));
        Assert.That (aee.DeniedAccessTypes, Is.EquivalentTo (new[] { ReadAccessType, DeleteAccessType }));
      }
    }


    [Test]
    [Explicit]
    public void NonContributingAcesDebugTestLarge ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup ("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, otherTenantGroup, otherTenantPosition);

      //var user2 = TestHelper.CreateUser ("NonContributingAcesDebugTest2", "User", "Other", "Chief", otherTenantGroup, otherTenant);

      var aceSpecificTenantWithOtherTenant = TestHelper.CreateAceWithSpecificTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (aceSpecificTenantWithOtherTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      To.ConsoleLine.e ("aceSpecificTenantWithOtherTenant",aceSpecificTenantWithOtherTenant.ToString ());
      To.ConsoleLine.e ("aceGroupOwning", aceGroupOwning.ToString ());

      //var userList = List.New (otherTenantUser, User);
      var userList = List.New (User);
      var aclList = List.New (TestHelper.CreateStatefulAcl (aceSpecificTenantWithOtherTenant, aceGroupOwning));

      OutputAccessStatistics (userList, aclList);

      //To.ConsoleLine.e (() => otherTenantAceSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      To.ConsoleLine.nl ().e (() => aclExpansionEntryList);
    }



    //--------------------------------------------------------------------------------------------------------------------------------
    // 2008-11-17 New SecurityManager ACE Features Tests
    //--------------------------------------------------------------------------------------------------------------------------------

    [Test]
    public void SpecificGroupTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup ("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceSpecificGroup = TestHelper.CreateAceWithSpecificGroup (otherTenantGroup);
      AttachAccessTypeReadWriteDelete (aceSpecificGroup, null, true, null);
      Assert.That (aceSpecificGroup.Validate ().IsValid);

      var aceOwningGroup = TestHelper.CreateAceWithOwningGroup ();
      AttachAccessTypeReadWriteDelete (aceOwningGroup, null, null, true);
      Assert.That (aceOwningGroup.Validate ().IsValid);

      //To.ConsoleLine.e (() => aceSpecificGroup);

      var userList = List.New (otherTenantUser);
      var aclList = List.New (TestHelper.CreateStatefulAcl (aceSpecificGroup, aceOwningGroup));

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      //To.ConsoleLine.e (() => aclExpansionEntryList);

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (2));

      // Test of specific-group-ACE: gives write-access
      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions ());

      //To.ConsoleLine.e (aclExpansionEntryList[1].AccessConditions.GroupHierarchyCondition);

      // Test of owning-group-ACE (specific-group-ACE matches also): gives write-access + delete-access with condition: group-must-own
      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType },
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[1].Role.Group, 
          GroupHierarchyCondition = GroupHierarchyCondition.This }
      );
    }

    [Test]
    public void SpecificGroupTypeTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      //var otherTenantParentGroup = TestHelper.CreateGroup ("ParentGroupForOtherTenant", null, otherTenant);

      GroupType groupType1 = TestHelper.CreateGroupType ("GroupType1");
      GroupType groupType2 = TestHelper.CreateGroupType ("GroupType2");

      var groupWithGroupType1 = TestHelper.CreateGroup ("GroupWithGroupType1", null, otherTenant, groupType1);
      var groupWithGroupType2 = TestHelper.CreateGroup ("GroupWithGroupType2", null, otherTenant, groupType2);
      var anotherGroupWithGroupType1 = TestHelper.CreateGroup ("AnotherGroupWithGroupType1", null, otherTenant, groupType1);

      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", groupWithGroupType1, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, groupWithGroupType1, otherTenantPosition);
      TestHelper.CreateRole (otherTenantUser, groupWithGroupType2, otherTenantPosition);
      TestHelper.CreateRole (otherTenantUser, anotherGroupWithGroupType1, otherTenantPosition);

      var aceSpecificGroupType1 = TestHelper.CreateAceWithSpecificGroupType (groupType1);
      AttachAccessTypeReadWriteDelete (aceSpecificGroupType1, null, true, null);
      Assert.That (aceSpecificGroupType1.Validate ().IsValid);

      var aceSpecificGroupType2 = TestHelper.CreateAceWithSpecificGroupType (groupType2);
      AttachAccessTypeReadWriteDelete (aceSpecificGroupType2, null, null, true);
      Assert.That (aceSpecificGroupType2.Validate ().IsValid);

      //To.ConsoleLine.e (() => aceSpecificGroup);

      var userList = List.New (otherTenantUser);
      var aclList = List.New (TestHelper.CreateStatefulAcl (aceSpecificGroupType1, aceSpecificGroupType2));

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      To.ConsoleLine.e (() => aclExpansionEntryList);

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions ());
      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { DeleteAccessType }, new AclExpansionAccessConditions ());
      AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { WriteAccessType }, new AclExpansionAccessConditions ());
    }


    [Test]
    public void GroupHierarchyConditionTest ()
    {
      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserPositionGroupSelection (User, Position, GroupCondition.OwningGroup, GroupHierarchyCondition.ThisAndParent);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };

      var owningGroup = aclExpansionEntryList[0].Role.Group;
      var groupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;

      var accessConditions = new AclExpansionAccessConditions ()
      {
        OwningGroup = owningGroup, //  GroupSelection.OwningGroup => group must be owner 
        GroupHierarchyCondition = groupHierarchyCondition
      };
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions.OwningGroup, Is.EqualTo (owningGroup));
      Assert.That (aclExpansionEntryList[0].AccessConditions.GroupHierarchyCondition, Is.EqualTo (groupHierarchyCondition));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }




    [Test]
    public void BranchOfOwningGroupTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      //var otherTenantParentGroup = TestHelper.CreateGroup ("ParentGroupForOtherTenant", null, otherTenant);

      GroupType groupType = TestHelper.CreateGroupType ("GroupType");
      GroupType groupTypeInAce = TestHelper.CreateGroupType ("GroupTypeInAce");

      var groupWithGroupTypeInAce = TestHelper.CreateGroup ("GroupWithGroupTypeInAce", null, otherTenant, groupTypeInAce);
      var group = TestHelper.CreateGroup ("Group", groupWithGroupTypeInAce, otherTenant, groupType);

      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", group, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, group, otherTenantPosition);
      //TestHelper.CreateRole (otherTenantUser, groupWithGroupType2, otherTenantPosition);
      //TestHelper.CreateRole (otherTenantUser, anotherGroupWithGroupType1, otherTenantPosition);

      var aceWithBranchOfOwningGroup = TestHelper.CreateAceWithBranchOfOwningGroup (groupTypeInAce);
      AttachAccessTypeReadWriteDelete (aceWithBranchOfOwningGroup, null, true, null);
      Assert.That (aceWithBranchOfOwningGroup.Validate ().IsValid);

      // Negative test: This ACE should not match
      var aceSpecificGroupType2 = TestHelper.CreateAceWithSpecificGroupType (groupTypeInAce);
      AttachAccessTypeReadWriteDelete (aceSpecificGroupType2, null, null, true);
      Assert.That (aceSpecificGroupType2.Validate ().IsValid);

      // Negative test: This ACE should not match
      var aceSpecificGroupType3 = TestHelper.CreateAceWithSpecificGroup (groupWithGroupTypeInAce);
      AttachAccessTypeReadWriteDelete (aceSpecificGroupType3, true, null, null);
      Assert.That (aceSpecificGroupType3.Validate ().IsValid);
 
      
      //To.ConsoleLine.e (() => aceSpecificGroup);

      var userList = List.New (otherTenantUser);
      var aclList = List.New (TestHelper.CreateStatefulAcl (aceWithBranchOfOwningGroup, aceSpecificGroupType2, aceSpecificGroupType3));

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      To.ConsoleLine.e (() => aclExpansionEntryList);

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      AssertAclExpansionEntry (aclExpansionEntryList[0], 
        new[] { WriteAccessType }, 
        new AclExpansionAccessConditions  {
          OwningGroup = groupWithGroupTypeInAce,
          GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren 
        }
      );
    }



    public void WriteAclExpansionAsHtmlSpikeToStreamWriter (List<AclExpansionEntry> aclExpansion, bool outputRowCount)
    {
      string aclExpansionFileName = "c:\\temp\\AclExpansionTest_" + FileNameTimestamp (DateTime.Now) + ".html";
      using (var streamWriter = new StreamWriter (aclExpansionFileName))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, streamWriter, true);
        aclExpansionHtmlWriter.Settings.OutputRowCount = outputRowCount;
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
      }
    }

    private string FileNameTimestamp (DateTime dt)
    {
      return StringUtility.ConcatWithSeparator (new[] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond }, "_");
    }




    /// <summary>
    /// NUnit-Asserts that the passed <see cref="AclExpansionEntry"/> has the passed <see cref="AccessTypeDefinition"/>|s and the
    /// passed <see cref="AclExpansionAccessConditions"/>.
    /// </summary>
    private void AssertAclExpansionEntry (AclExpansionEntry actualAclExpansionEntry, AccessTypeDefinition[] expectedAccessTypeDefinitions,
      AclExpansionAccessConditions expectedAclExpansionAccessConditions)
    {
      Assert.That (actualAclExpansionEntry.AllowedAccessTypes, Is.EquivalentTo (expectedAccessTypeDefinitions));
      Assert.That (actualAclExpansionEntry.AccessConditions, Is.EqualTo (expectedAclExpansionAccessConditions));
    }


 

    // Returns a list of AclExpansionEntry for the passed User, ACE with the passed Positon and passed GroupSelection
    private List<AclExpansionEntry> GetAclExpansionEntryList_UserPositionGroupSelection (
      User user, Position position, GroupCondition groupCondition, GroupHierarchyCondition groupHierarchyCondition)
    {
      List<User> userList = new List<User> ();
      userList.Add (user);

      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); 
      userFinderMock.Expect (mock => mock.FindUsers()).Return (userList);

      List<AccessControlList> aclList = new List<AccessControlList>();

      //var ace = TestHelper.CreateAceWithPosition (position, groupCondition);
      var ace = TestHelper.CreateAceWithPosition (position, groupCondition);
      //ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      ace.GroupHierarchyCondition = groupHierarchyCondition;

      AttachAccessTypeReadWriteDelete (ace, true, null, true);
      Assert.That (ace.Validate ().IsValid);
      var acl = TestHelper.CreateStatefulAcl (ace);
      aclList.Add (acl);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList();
      //To.ConsoleLine.e (() => aclExpansionEntryList);
      userFinderMock.VerifyAllExpectations();
      aclFinderMock.VerifyAllExpectations ();
      return aclExpansionEntryList;
    }


    private void AssertIsNotInMatchingAces (List<User> userList, List<AccessControlList> aclList)
    {
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> ();
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (userList);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      foreach (User user in userList)
      {
        foreach (Role role in user.Roles)
        {
          foreach (AccessControlList acl in aclList)
          {
            foreach (AccessControlEntry ace in acl.AccessControlEntries)
            {
              AclProbe aclProbe;
              AccessTypeStatistics accessTypeStatistics;
              aclExpander.GetAccessTypes (new UserRoleAclAceCombination (role, ace), out aclProbe, out accessTypeStatistics);
              Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.False);
            }
          }
        }
      }
    }


    private void OutputAccessStatistics (List<User> userList, List<AccessControlList> aclList)
    {
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> ();
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (userList);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      foreach (User user in userList)
      {
        foreach (Role role in user.Roles)
        {
          foreach (AccessControlList acl in aclList)
          {
            foreach (AccessControlEntry ace in acl.AccessControlEntries)
            {
              AclProbe aclProbe;
              AccessTypeStatistics accessTypeStatistics;
              aclExpander.GetAccessTypes (new UserRoleAclAceCombination (role, ace), out aclProbe, out accessTypeStatistics);
             // Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.False);
              To.ConsoleLine.s ("--------------------------------------------------------------------------------");
              To.ConsoleLine.sb ().e (() => user).e (() => role).e (() => ace).e (() => acl).se();
              To.ConsoleLine.e ("MatchingAces", accessTypeStatistics.MatchingAces);
              To.ConsoleLine.e ("AccessTypesSupplyingAces", accessTypeStatistics.AccessTypesSupplyingAces);
            }
          }
        }
      }
    }

  }
}

