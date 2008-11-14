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

      TestHelper.CreateAceWithSpecficTenant (Tenant);
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
        GetAclExpansionEntryList_UserPositionGroupSelection(User,Position,GroupCondition.None);

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
        GetAclExpansionEntryList_UserPositionGroupSelection (User, Position, GroupCondition.OwningGroup);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions () { 
        IsOwningGroupRequired = true, //  GroupSelection.OwningGroup => group must be owner
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
        IsOwningGroupRequired = true, //  GroupSelection.OwningGroup => group must be owner
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
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });

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

      var aceGroupSpecificTenant = TestHelper.CreateAceWithSpecficTenant (otherTenant);
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

      var aceGroupSpecificTenant = TestHelper.CreateAceWithSpecficTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (aceGroupSpecificTenant, null, true, null);

      var aceGroupOwningTenant = TestHelper.CreateAceWithOwningTenant ();
      AttachAccessTypeReadWriteDelete (aceGroupOwningTenant, null, null, true);

      //To.ConsoleLine.e (() => aceGroupSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (otherTenantUser),
          List.New (TestHelper.CreateStatefulAcl (aceGroupSpecificTenant, aceGroupOwningTenant))
        );


      To.ConsoleLine.e (() => aclExpansionEntryList);

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

      var aceSpecificTenantWithOtherTenant = TestHelper.CreateAceWithSpecficTenant (otherTenant);
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
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });
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
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });
    }



    [Test]
    public void GetAclExpansionEntryList_UserList_SeparateAcls ()
    {
      //var aceMatchAll = TestHelper.CreateAceWithGroupSelectionAll();
      //AttachAccessTypeReadWriteDelete (aceMatchAll, true, null, null);

      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant ();
      AttachAccessTypeReadWriteDelete (aceOwningTenant, true, true, null);

      var aceSpecificTenant = TestHelper.CreateAceWithSpecficTenant (Tenant);
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
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });

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
      var testAce = TestHelper.CreateAceWithSpecficTenant (otherTenant);
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
      OutputAccessStatistics (userList, aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (userList, aclList);

      // To.ConsoleLine.nl().e (() => aclExpansionEntryList); To.ConsoleLine.nl().e (User2.Roles);

      // Note: Without "current role only"-probing in AclExpander.GetAccessTypes, the role {"Anotha Group","Working Drone"}
      // would also give an aclExpansionEntryList entry.
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));


      var aclExpansion = aclExpansionEntryList[0];

      AssertAclExpansionEntry (aclExpansion, new[] { ReadAccessType, WriteAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });

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

      var aceSpecificTenantWithOtherTenant = TestHelper.CreateAceWithSpecficTenant (otherTenant);
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
      User user, Position position, GroupCondition groupCondition)
    {
      List<User> userList = new List<User> ();
      userList.Add (user);

      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); 
      userFinderMock.Expect (mock => mock.FindUsers()).Return (userList);

      List<AccessControlList> aclList = new List<AccessControlList>();
      var ace = TestHelper.CreateAceWithPosition (position, groupCondition);
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
