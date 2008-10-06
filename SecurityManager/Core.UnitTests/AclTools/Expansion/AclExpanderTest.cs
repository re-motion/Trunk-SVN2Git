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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.ObjectMother;

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
      _aclList.Add (TestHelper.CreateAcl());
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

      AccessControlList acl = TestHelper.CreateAcl (ace);
      aclList.Add (acl);

      SecurityToken securityToken = new SecurityToken (user, User.Tenant, new List<Group> (), new List<AbstractRoleDefinition> ());
      AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (securityToken);
      To.ConsoleLine.sb ().e (accessTypeDefinitions.Length).e (() => accessTypeDefinitions).se ();
    }


    [Test]
    public void AccessControlList_GetAccessTypes2 ()
    {
      var user = User3;
      //var acl = TestHelper.CreateAcl (Ace3, Ace2, Ace);
      var acl = TestHelper.CreateAcl (Ace3);
      Assert.That (Ace3.Validate().IsValid);
      SecurityToken securityToken = new SecurityToken (user, user.Tenant, new List<Group> (), new List<AbstractRoleDefinition> ());
      AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (securityToken);
      //To.ConsoleLine.s ("AccessControlList_GetAccessTypes2: ").sb ().e (() => accessTypeDefinitions).se ();

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, WriteAccessType };
      Assert.That (accessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitionsExpected));
    }

    [Test]
    public void AccessControlList_GetAccessTypes_AceWithPosition_GroupSelectionAll ()
    {
      var ace = TestHelper.CreateAceWithPosition (Position, GroupSelection.All);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);

      Assert.That (ace.Validate ().IsValid);
      
      var acl = TestHelper.CreateAcl (ace);
      SecurityToken securityToken = new SecurityToken (User, User.Tenant, new List<Group> (), new List<AbstractRoleDefinition> ());
      AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (securityToken);
   
      To.ConsoleLine.s ("AccessControlList_GetAccessTypes2: ").sb ().e (() => accessTypeDefinitions).se ();

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      Assert.That (accessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitionsExpected));
    }

    [Test]
    public void AccessControlList_GetAccessTypes_AceWithPosition_GroupSelectionOwningGroup ()
    {
      var ace = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);

      Assert.That (ace.Validate ().IsValid);

      var acl = TestHelper.CreateAcl (ace);
      // We pass the Group used in the ace Position above in the owningGroups-list => ACE will match.
      SecurityToken securityToken = new SecurityToken (User, User.Tenant, List.New (Group), new List<AbstractRoleDefinition> ());
      AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (securityToken);

      To.ConsoleLine.s ("AccessControlList_GetAccessTypes2: ").sb ().e (() => accessTypeDefinitions).se ();

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      Assert.That (accessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitionsExpected));
    }



    [Test]
    public void GetAclExpansionEntryList_AceWithPosition_GroupSelectionAll ()
    {
      List<AclExpansionEntry> aclExpansionEntryList = 
        GetAclExpansionEntryList_UserPositionGroupSelection(User,Position,GroupSelection.All);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions();
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AccessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }

    [Test]
    public void GetAclExpansionEntryList_AceWithPosition_GroupSelectionOwningGroup ()
    {
      List<AclExpansionEntry> aclExpansionEntryList = 
        GetAclExpansionEntryList_UserPositionGroupSelection (User, Position, GroupSelection.OwningGroup);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions () { 
        OnlyIfGroupIsOwner = true, //  GroupSelection.OwningGroup => group must be owner
      };
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AccessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }

    [Test]
    public void GetAclExpansionEntryList_UserList_AceList ()
    {
      var ace = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);
      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (User),
          List.New(TestHelper.CreateAcl(ace))  
        );

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions ()
      {
        OnlyIfGroupIsOwner = true, //  GroupSelection.OwningGroup => group must be owner
      };
      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));
      Assert.That (aclExpansionEntryList[0].AccessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitionsExpected));
      Assert.That (aclExpansionEntryList[0].AccessConditions, Is.EqualTo (accessConditions));
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_MultipleAces ()
    {
      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      var aceAbstractRole = TestHelper.CreateAceWithAbstractRole ();
      AttachAccessTypeReadWriteDelete (aceAbstractRole, null, true, true);

      var aceGroupAll = TestHelper.CreateAceWithGroupSelectionAll ();
      AttachAccessTypeReadWriteDelete (aceGroupAll, null, true, null);

      //To.ConsoleLine.e (() => aceGroupOwning);
      //To.ConsoleLine.e (() => aceAbstractRole);
      //To.ConsoleLine.e (() => aceGroupAll);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (User),
          List.New (TestHelper.CreateAcl (aceGroupOwning, aceAbstractRole, aceGroupAll))
        );

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, DeleteAccessType }, 
        new AclExpansionAccessConditions { OnlyIfGroupIsOwner = true });

      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType },
       new AclExpansionAccessConditions { AbstractRole = aceAbstractRole.SpecificAbstractRole });

      AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { WriteAccessType },
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


      To.ConsoleLine.e (() => aceGroupSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          List.New (otherTenantUser),
          List.New (TestHelper.CreateAcl (aceGroupSpecificTenant))
        );


      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { WriteAccessType },
        new AclExpansionAccessConditions());
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

      var otherTenantAceSpecificTenant = TestHelper.CreateAceWithSpecficTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (otherTenantAceSpecificTenant, true, true, true);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      To.ConsoleLine.e (() => otherTenantAceSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList (
          //List.New (otherTenantUser, User),
          List.New (otherTenantUser),
          List.New (TestHelper.CreateAcl (otherTenantAceSpecificTenant, aceGroupOwning))
        );


      //Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

      //AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, DeleteAccessType },
      //  new AclExpansionAccessConditions { OnlyIfGroupIsOwner = true });

      //AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { WriteAccessType },
      //  new AclExpansionAccessConditions ());

      //AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { ReadAccessType, DeleteAccessType },
      //  new AclExpansionAccessConditions { OnlyIfGroupIsOwner = true });
    }


    /// <summary>
    /// NUnit-Asserts that the passed <see cref="AclExpansionEntry"/> has the passed <see cref="AccessTypeDefinition"/>|s and the
    /// passed <see cref="AclExpansionAccessConditions"/>.
    /// </summary>
    private void AssertAclExpansionEntry (AclExpansionEntry aclExpansionEntry, AccessTypeDefinition[] accessTypeDefinitions,
      AclExpansionAccessConditions aclExpansionAccessConditions)
    {
      Assert.That (aclExpansionEntry.AccessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitions));
      Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (aclExpansionAccessConditions));
    }


    private List<AclExpansionEntry> GetAclExpansionEntryList_UserList_AceList (
      List<User> userList, List<AccessControlList> aclList)
    {
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); //new TestAclExpanderUserFinder (userList);
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (userList);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList_Spike ();
      To.ConsoleLine.e (() => aclExpansionEntryList);
      userFinderMock.VerifyAllExpectations ();
      aclFinderMock.VerifyAllExpectations ();
      return aclExpansionEntryList;
    }




    // Returns a list of AclExpansionEntry for the passed User, ACE with the passed Positon and passed GroupSelection
    private List<AclExpansionEntry> GetAclExpansionEntryList_UserPositionGroupSelection (
      User user, Position position, GroupSelection groupSelection)
    {
      List<User> userList = new List<User> ();
      userList.Add (user);

      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); //new TestAclExpanderUserFinder (userList);
      userFinderMock.Expect (mock => mock.FindUsers()).Return (userList);

      List<AccessControlList> aclList = new List<AccessControlList>();
      var ace = TestHelper.CreateAceWithPosition (position, groupSelection);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);
      Assert.That (ace.Validate ().IsValid);
      var acl = TestHelper.CreateAcl (ace);
      aclList.Add (acl);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList_Spike();
      To.ConsoleLine.e (() => aclExpansionEntryList);
      userFinderMock.VerifyAllExpectations();
      aclFinderMock.VerifyAllExpectations ();
      return aclExpansionEntryList;
    }
  }


  [ToTextSpecificHandler]
  public class AbstractRoleDefinition_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AbstractRoleDefinition>
  {
    public override void ToText (AbstractRoleDefinition x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AbstractRoleDefinition> ("").e (x.Name).ie ();
    }
  }


  [ToTextSpecificHandler]
  public class Permission_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Permission>
  {
    public override void ToText (Permission x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<Permission> ("").e (x.AccessType).e (x.Allowed).ie ();
    }
  }


  [ToTextSpecificHandler]
  public class Position_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Position>
  {
    public override void ToText (Position x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<Position> ("").e (x.DisplayName).ie ();
    }
  }


  [ToTextSpecificHandler]
  public class AccessControlEntry_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessControlEntry>
  {
    public override void ToText (AccessControlEntry x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AccessControlEntry> ("").e (x.Permissions).e ("SelUser", x.UserSelection).e ("SelGroup", x.GroupSelection).e ("SelTenant", x.TenantSelection).eIfNotNull ("user", x.SpecificUser).eIfNotNull ("position", x.SpecificPosition).eIfNotNull ("group", x.SpecificGroup).eIfNotNull ("tenant", x.SpecificTenant).ie ();
    }
  }

  [ToTextSpecificHandler]
  public class AccessControlList_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessControlList>
  {
    public override void ToText (AccessControlList x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AccessControlList> ("").e (x.AccessControlEntries).ie ();
    }
  }


  [ToTextSpecificHandler]
  public class ListOfAclExpansionEntry_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<List<AclExpansionEntry>>
  {
    public override void ToText (List<AclExpansionEntry> listOfAclExpansionEntry, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<List<AclExpansionEntry>> ("").nl();
      foreach (AclExpansionEntry aclExpansionEntry in listOfAclExpansionEntry)
      {
        toTextBuilder.e (aclExpansionEntry).nl ();
      }
      toTextBuilder.ie ();
    }
  }


  [ToTextSpecificHandler]
  public class AclProbe_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AclProbe>
  {
    public override void ToText (AclProbe x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AclProbe> ("").e("token",x.SecurityToken).e("conditions",x.AccessConditions) .ie ();
    }
  }

  [ToTextSpecificHandler]
  public class Tenant_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Tenant>
  {
    public override void ToText (Tenant x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<Tenant> ("").e (x.DisplayName).ie ();
    }
  }

  [ToTextSpecificHandler]
  public class SecurityToken_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<SecurityToken>
  {
    public override void ToText (SecurityToken x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<SecurityToken> ("").e ("principal", x.User).eIfNotNull (x.OwningTenant).eIfNotNull (x.OwningGroups).eIfNotNull (x.OwningGroupRoles).eIfNotNull (x.AbstractRoles).ie ();
    }
  }
}