// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Rhino.Mocks;

// TODO AE: Cleanup usings.

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  // TODO AE: Remove commented code. (Do not commit.)
  public class AclToolsTestBase : DomainTest
  {
    // TODO AE: Rename properties to express semantics (eg. AceWithOwningTenant and AceWithSpecificTenant rather than Ace and Ace2).
    public StatefulAccessControlList Acl { get; private set; }
    public StatefulAccessControlList Acl2 { get; private set; }
    
    public AccessTypeDefinition DeleteAccessType { get; private set; }
    public AccessTypeDefinition WriteAccessType { get; private set; }
    public AccessTypeDefinition ReadAccessType { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions { get; private set; }
    public AccessControlTestHelper TestHelper { get; private set; }
    public Tenant Tenant { get; private set; }
    public Group Group { get; private set; }
    public Position Position { get; private set; }
    public Role Role { get; private set; }
    public User User { get; private set; }
    public AccessControlEntry Ace { get; private set; }

    public AccessTypeDefinition[] AccessTypeDefinitions2 { get; private set; }
    public AccessControlEntry Ace2 { get; private set; }
    public Role Role2 { get; private set; }
    public User User2 { get; private set; }
    public Position Position2 { get; private set; }
    public Group Group2 { get; private set; }

    //public AccessTypeDefinition[] AccessTypeDefinitions3 { get; private set; }
    public AccessControlEntry Ace3 { get; private set; }
    public Role Role3 { get; private set; }
    public User User3 { get; private set; }
    public Position Position3 { get; private set; }
    public Group Group3 { get; private set; }

    // Called before each test gets executed.
    // [SetUp] // TODO AE: Add attribute
    public override void SetUp ()
    {
      base.SetUp ();
      TestHelper = new AccessControlTestHelper ();

      // Base class TearDown()-method (executed after each test) calls ClientTransactionScope.ResetActiveScope(),
      // discarding the transaction opened by EnterNonDiscardingScope below. // TODO AE: Why not close the scope manually in TearDown? (Better style, and gets rid of the comment.)
      TestHelper.Transaction.EnterNonDiscardingScope ();


      ReadAccessType = TestHelper.CreateReadAccessType ();  // read access
      WriteAccessType = TestHelper.CreateWriteAccessType ();  // write access
      DeleteAccessType = TestHelper.CreateDeleteAccessType ();  // delete permission

      AccessTypeDefinitions = new[] { ReadAccessType, WriteAccessType, DeleteAccessType };
      AccessTypeDefinitions2 = new[] { ReadAccessType, DeleteAccessType };


      Tenant = TestHelper.CreateTenant ("Da Tenant");
      Group = TestHelper.CreateGroup ("Da Group", null, Tenant);
      Position = TestHelper.CreatePosition ("Supreme Being");
      User = TestHelper.CreateUser ("DaUs", "Da", "Usa", "Dr.", Group, Tenant);
      Role = TestHelper.CreateRole (User, Group, Position);
      Ace = TestHelper.CreateAceWithOwningTenant();

      TestHelper.AttachAccessType (Ace, ReadAccessType, null);
      TestHelper.AttachAccessType (Ace, WriteAccessType, true);
      TestHelper.AttachAccessType (Ace, DeleteAccessType, null);


      Group2 = TestHelper.CreateGroup ("Anotha Group", null, Tenant);
      Position2 = TestHelper.CreatePosition ("Working Drone");
      User2 = TestHelper.CreateUser ("mr.smith", "", "Smith", "Mr.", Group2, Tenant);
      Role2 = TestHelper.CreateRole (User2, Group2, Position2);
      Ace2 = TestHelper.CreateAceWithSpecificTenant (Tenant);

      TestHelper.AttachAccessType (Ace2, ReadAccessType, true);
      TestHelper.AttachAccessType (Ace2, WriteAccessType, null);
      TestHelper.AttachAccessType (Ace2, DeleteAccessType, true);


      Group3 = TestHelper.CreateGroup ("Da 3rd Group", null, Tenant);
      Position3 = TestHelper.CreatePosition ("Combatant");
      User3 = TestHelper.CreateUser ("ryan_james", "Ryan", "James", "", Group3, Tenant);
      Role3 = TestHelper.CreateRole (User3, Group3, Position3);
      // TODO AE: Maybe fix functionality for group matching instead?
      // DO NOT use TestHelper.CreateAceWithOwningGroup() here; functionality for group matching is
      // incomplete and therefore the ACE entry will always match.
      Ace3 = TestHelper.CreateAceWithPositionAndGroupCondition (Position3, GroupCondition.None);

      TestHelper.AttachAccessType (Ace3, ReadAccessType, true);
      TestHelper.AttachAccessType (Ace3, WriteAccessType, true);
      TestHelper.AttachAccessType (Ace3, DeleteAccessType, null);


      // TODO AE: Consider dropping this heading.
      //--------------------------------
      // Create ACLs
      //--------------------------------

      //Acl = TestHelper.CreateAcl (Ace, Ace2, Ace3);

      SecurableClassDefinition orderClass = SecurableClassDefinition.GetObject (SetUpFixture.OrderClassID);
      //var aclList = SecurableClassDefinition.GetObject (SetUpFixture.OrderClassID).AccessControlLists;
      var aclList = orderClass.StatefulAccessControlLists;
      Assert.That (aclList.Count, Is.GreaterThanOrEqualTo (2));
      
      Acl = aclList[0];
      TestHelper.AttachAces (Acl, Ace, Ace2, Ace3);

      var ace2_1 = TestHelper.CreateAceWithAbstractRole();
      var ace2_2 = TestHelper.CreateAceWithPositionAndGroupCondition (Position2, GroupCondition.OwningGroup);

      //Acl2 = TestHelper.CreateAcl (ace2_1, ace2_2, Ace3);
      Acl2 = aclList[1];
      TestHelper.AttachAces (Acl2, ace2_1, ace2_2, Ace3);




      // Additional roles for users
      //TestHelper.CreateRole (User, Group2, Position2);
      //TestHelper.CreateRole (User, Group3, Position3);

      TestHelper.CreateRole (User2, Group, Position2);
      TestHelper.CreateRole (User2, Group2, Position);
      TestHelper.CreateRole (User2, Group3, Position2);

      TestHelper.CreateRole (User3, Group, Position);
      TestHelper.CreateRole (User3, Group2, Position2);
      TestHelper.CreateRole (User3, Group3, Position3);
      TestHelper.CreateRole (User3, Group, Position3);
      TestHelper.CreateRole (User3, Group2, Position);

    }

    public void AttachAccessTypeReadWriteDelete (AccessControlEntry ace, bool? allowRead, bool? allowWrite, bool? allowDelete)
    {
      TestHelper.AttachAccessType (ace, ReadAccessType, allowRead);
      TestHelper.AttachAccessType (ace, WriteAccessType, allowWrite);
      TestHelper.AttachAccessType (ace, DeleteAccessType, allowDelete);
    }


    // TODO AE: Remove unused method
    private User CreateUser (Tenant userTenant, Group userGroup)
    {
      return TestHelper.CreateUser ("JoDo", "John", "Doe", "Prof.", userGroup, userTenant);
    }


    // TODO AE: Consider renaming method to just "GetAclExpansionEntryList".
    protected List<AclExpansionEntry> GetAclExpansionEntryList_UserList_AceList (List<User> userList, List<AccessControlList> aclList, 
      bool sortedAndDistinct)
    {
      // TODO AE: Use stubs rather than mocks if verification is not the goal.
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> ();
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (userList);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (aclList);

      var aclExpander = new AclExpander (userFinderMock, aclFinderMock);
      
      List<AclExpansionEntry> aclExpansionEntryList; 

      if (sortedAndDistinct)
      {
        aclExpansionEntryList = aclExpander.GetAclExpansionEntryListSortedAndDistinct ();
      }
      else
      {
        aclExpansionEntryList = aclExpander.GetAclExpansionEntryList().ToList();
      }
      //To.ConsoleLine.e (() => aclExpansionEntryList);
      userFinderMock.VerifyAllExpectations ();
      aclFinderMock.VerifyAllExpectations ();
      return aclExpansionEntryList;
    }

  }
}
