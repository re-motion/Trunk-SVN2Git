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
using System.Linq;
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
  public class AclExpanderTest_Spike : AclToolsTestBase
  {
    private readonly List<AccessControlList> _aclList = new List<AccessControlList> ();
    private readonly List<User> _userList = new List<User> ();

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      TestHelper.CreateAceWithSpecficTenant (Tenant);
      _aclList.Add (TestHelper.CreateAcl ());
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
      Assert.That (Ace3.Validate ().IsValid);
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
        GetAclExpansionEntryList_UserPositionGroupSelection (User, Position, GroupSelection.All);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions ();
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
      var accessConditions = new AclExpansionAccessConditions ()
      {
        IsOwningGroupRequired = true, //  GroupSelection.OwningGroup => group must be owner
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
        GetAclExpansionEntryList_UserList_AceList_Extended (
          List.New (User),
          List.New (TestHelper.CreateAcl (ace))
        );

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions ()
      {
        IsOwningGroupRequired = true, //  GroupSelection.OwningGroup => group must be owner
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
        GetAclExpansionEntryList_UserList_AceList_Extended (
          List.New (User),
          List.New (TestHelper.CreateAcl (aceGroupOwning, aceAbstractRole, aceGroupAll))
        );

      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });

      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType },
       new AclExpansionAccessConditions { AbstractRole = aceAbstractRole.SpecificAbstractRole });

      AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { WriteAccessType },
       new AclExpansionAccessConditions ());
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
        GetAclExpansionEntryList_UserList_AceList_Extended (
          List.New (otherTenantUser),
          List.New (TestHelper.CreateAcl (aceGroupSpecificTenant))
        );


      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (1));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { WriteAccessType },
        new AclExpansionAccessConditions ());
    }



    [Test]
    //[Ignore("Fixme to accept double AclExpansion entries.")]
    public void GetAclExpansionEntryList_UserList_AceList_TwoDifferentTenants ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant ("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup ("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition ("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser ("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      var otherTenantRole = TestHelper.CreateRole (otherTenantUser, otherTenantGroup, otherTenantPosition);

      var otherTenantAceSpecificTenant = TestHelper.CreateAceWithSpecficTenant (otherTenant);
      AttachAccessTypeReadWriteDelete (otherTenantAceSpecificTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      //To.ConsoleLine.e (() => otherTenantAceSpecificTenant);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList_Extended (
          List.New (otherTenantUser, User),
        //List.New (otherTenantUser),
          List.New (TestHelper.CreateAcl (otherTenantAceSpecificTenant, aceGroupOwning))
        );


      Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

      AssertAclExpansionEntry (aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions ());

      AssertAclExpansionEntry (aclExpansionEntryList[1], new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });

      AssertAclExpansionEntry (aclExpansionEntryList[2], new[] { ReadAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { IsOwningGroupRequired = true });
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_MultipleAces ()
    {
      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant ();
      AttachAccessTypeReadWriteDelete (aceOwningTenant, true, true, null);

      var aceSpecificTenant = TestHelper.CreateAceWithSpecficTenant (Tenant);
      AttachAccessTypeReadWriteDelete (aceSpecificTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList_Extended (
          List.New (User),
          List.New (TestHelper.CreateAcl (aceOwningTenant, aceSpecificTenant, aceGroupOwning))
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
    public void GetAclExpansionEntryList_UserList_SeparateAcls ()
    {
      //var aceMatchAll = TestHelper.CreateAceWithGroupSelectionAll();
      //AttachAccessTypeReadWriteDelete (aceMatchAll, true, null, null);

      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant ();
      AttachAccessTypeReadWriteDelete (aceOwningTenant, true, true, null);

      var aceSpecificTenant = TestHelper.CreateAceWithSpecficTenant (Tenant);
      AttachAccessTypeReadWriteDelete (aceSpecificTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPosition (Position, GroupSelection.OwningGroup);
      AttachAccessTypeReadWriteDelete (aceGroupOwning, true, null, true);

      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserList_AceList_Extended (
          List.New (User),
        //List.New (TestHelper.CreateAcl (aceMatchAll, aceSpecificTenant, aceGroupOwning))
          List.New (TestHelper.CreateAcl (aceOwningTenant), TestHelper.CreateAcl (aceSpecificTenant), TestHelper.CreateAcl (aceGroupOwning))
        );


      //Assert.That (aclExpansionEntryList.Count, Is.EqualTo (3));

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




    /// <summary>
    /// NUnit-asserts that the passed <see cref="AclExpansionEntry"/> has the passed <see cref="AccessTypeDefinition"/>|s and the
    /// passed <see cref="AclExpansionAccessConditions"/>.
    /// </summary>
    private void AssertAclExpansionEntry (AclExpansionEntry aclExpansionEntry, AccessTypeDefinition[] accessTypeDefinitions,
      AclExpansionAccessConditions aclExpansionAccessConditions)
    {
      Assert.That (aclExpansionEntry.AccessTypeDefinitions, Is.EquivalentTo (accessTypeDefinitions));
      Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (aclExpansionAccessConditions));
    }


    private List<AclExpansionEntry> GetAclExpansionEntryList_UserList_AceList_Extended (
      List<User> userList, List<AccessControlList> aclList)
    {
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); //new TestAclExpanderUserFinder (userList);
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (userList);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      //var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList_Spike ();
      var aclExpansionEntryList = GetAclExpansionEntryList_Spike (aclExpander, userFinderMock, aclFinderMock);
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
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (userList);

      List<AccessControlList> aclList = new List<AccessControlList> ();
      var ace = TestHelper.CreateAceWithPosition (position, groupSelection);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);
      Assert.That (ace.Validate ().IsValid);
      var acl = TestHelper.CreateAcl (ace);
      aclList.Add (acl);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      //var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList_Spike ();
      var aclExpansionEntryList = GetAclExpansionEntryList_Spike (aclExpander, userFinderMock, aclFinderMock);

      To.ConsoleLine.e (() => aclExpansionEntryList);
      userFinderMock.VerifyAllExpectations ();
      aclFinderMock.VerifyAllExpectations ();
      return aclExpansionEntryList;
    }

  
    // Spike implementation to test algorithm. 
    public static List<AclExpansionEntry> GetAclExpansionEntryList_Spike (AclExpander aclExpander, 
      IAclExpanderUserFinder _userFinder, IAclExpanderAclFinder _accessControlListFinder)
    {
      var aclExpansionEntries = new List<AclExpansionEntry>();

      var users = _userFinder.FindUsers();
      var acls = _accessControlListFinder.FindAccessControlLists();

      foreach (var user in users)
      {
        To.ConsoleLine.e (() => user);
        foreach (var role in user.Roles)
        {
          To.ConsoleLine.s ("\t").e (() => role);
          foreach (var acl in acls)
          {
            To.ConsoleLine.s ("\t\t").e (() => acl);
            foreach (var ace in acl.AccessControlEntries)
            {
              To.ConsoleLine.s ("\t\t\t").e (() => ace);
              AclProbe aclProbe = AclProbe.CreateAclProbe (user, role, ace);
              To.ConsoleLine.s ("\t\t\t").e (() => aclProbe);

              // TODO(?): Check if we already queried with an identical token.
              AccessTypeDefinition[] accessTypeDefinitions = acl.GetAccessTypes (aclProbe.SecurityToken);

              if (accessTypeDefinitions.Length > 0)
              {
                var aclExpansionEntry = new AclExpansionEntry (user, role, acl, aclProbe.AccessConditions, accessTypeDefinitions);
                To.ConsoleLine.s ("\t\t\t").e (() => aclExpansionEntry);
                aclExpansionEntries.Add (aclExpansionEntry);
              }
            }
          }
        }
      }

      List<AclExpansionEntry> aclExpansionEntriesSortedAndDistinct = 
        (from AclExpansionEntry aclExpansionEntry in aclExpansionEntries
                //orderby aclExpansionEntry.User.DisplayName, aclExpansionEntry.Role.DisplayName
                select aclExpansionEntry ).Distinct().ToList();


      return aclExpansionEntriesSortedAndDistinct;
    }


  }
}