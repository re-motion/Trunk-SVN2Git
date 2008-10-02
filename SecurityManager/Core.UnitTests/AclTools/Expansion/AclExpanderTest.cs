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

      //SecurableClassDefinition orderClass = TestHelper.CreateOrderClassDefinition ();
      //SecurableClassDefinition premiumOrderClass = TestHelper.CreatePremiumOrderClassDefinition (orderClass);
      //AccessControlList aclForOrder = TestHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      //AccessControlList aclForPremiumOrder = TestHelper.GetAclForDeliveredAndUnpaidAndDhlStates (premiumOrderClass);
      //SecurityContext context = CreateContextForDeliveredAndUnpaidAndDhlOrder (typeof (SpecialOrder));
      //AccessControlListFinder aclFinder = new AccessControlListFinder ();
      //AccessControlList foundAcl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, premiumOrderClass, context);

      TestHelper.CreateAceWithSpecficTenant (Tenant);
      _aclList.Add (TestHelper.CreateAcl());


    }


    /*
    [Test]
    public void FindMatchingEntries_WithMultipleMatchingAces ()
    {
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = TestHelper.CreateReadAccessTypeAndSetWithValueAtAce (ace1, true);
      AccessTypeDefinition writeAccessType = TestHelper.CreateWriteAccessTypeAndSetWithValueAtAce (ace1, null);
      AccessTypeDefinition deleteAccessType = TestHelper.CreateDeleteAccessTypeAndSetWithValueAtAce (ace1, null);

      AbstractRoleDefinition role2 = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.SpecificAbstractRole = role2;
      TestHelper.AttachAccessType (ace2, readAccessType, null);
      TestHelper.AttachAccessType (ace2, writeAccessType, true);
      TestHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = TestHelper.CreateAcl (ace1, ace2);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (role2);

      AccessControlEntry[] entries = acl.FindMatchingEntries (token);

      Assert.AreEqual (2, entries.Length);
      Assert.Contains (ace2, entries);
      Assert.Contains (ace1, entries);
    }
    */

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
    //[Ignore]
    public void GetAclExpansionEntryList ()
    {
      List<User> userList = new List<User> ();
      var user = User;
      userList.Add (user);

      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); //new TestAclExpanderUserFinder (userList);
      userFinderMock.Expect (mock => mock.FindUsers()).Return (userList);

      List<AccessControlList> aclList = new List<AccessControlList>();
      var ace = TestHelper.CreateAceWithPosition (Position, GroupSelection.All);
      AttachAccessTypeReadWriteDelete (ace, true, null, true);
      Assert.That (ace.Validate ().IsValid);
      var acl = TestHelper.CreateAcl (ace);
      aclList.Add (acl);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      //var ace = TestHelper.CreateAceWithAbstractRole();
      //ace.TenantSelection = TenantSelection.All;
      //ace.GroupSelection = GroupSelection.All;
      //ace.UserSelection = UserSelection.All;
      //var acl = TestHelper.CreateAcl (ace);
      //aclList.Add (acl);
      //var aclFinder = new TestAclExpanderAclFinder (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList_Spike();
      To.ConsoleLine.e (() => aclExpansionEntryList);
      userFinderMock.VerifyAllExpectations();
      aclFinderMock.VerifyAllExpectations ();
    }
  }

  public class TestAclExpanderAclFinder : IAclExpanderAclFinder
  {
    private readonly List<AccessControlList> _acls;
    public TestAclExpanderAclFinder (List<AccessControlList> acls) { _acls = acls; }

    public List<AccessControlList> FindAccessControlLists ()
    {
      return _acls;
    }
  }

  //public class TestAclExpanderUserFinder : IAclExpanderUserFinder
  //{
  //  private readonly List<User> _users;
  //  public TestAclExpanderUserFinder (List<User> users) { _users = users; }
  //  public List<User> Users { get { return _users; } }
  //}
}