// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Security;
using Remotion.Data.DomainObjects.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectCollection positions = Position.FindAll ();

        Assert.AreEqual (3, positions.Count);
      }
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("Position");
        AccessControlEntry ace = testHelper.CreateAceWithPositionAndGroupCondition (position, GroupCondition.None);

        position.Delete();

        Assert.IsTrue (ace.IsInvalid);
      }
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = testHelper.CreateTenant ("TestTenant", "UID: testTenant");
        Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid().ToString(), null, tenant);
        Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
        User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
        Position position = testHelper.CreatePosition ("Position");
        Role role = testHelper.CreateRole (user, roleGroup, position);

        position.Delete();

        Assert.IsTrue (role.IsInvalid);
      }
    }

    [Test]
    public void DeletePosition_WithGroupTypePosition ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = testHelper.CreateGroupType ("GroupType");
        Position position = testHelper.CreatePosition ("Position");
        GroupTypePosition concretePosition = testHelper.CreateGroupTypePosition (groupType, position);

        position.Delete();

        Assert.IsTrue (concretePosition.IsInvalid);
      }
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");

        Assert.AreEqual ("PositionName", position.DisplayName);
      }
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        IObjectSecurityStrategy objectSecurityStrategy = position.GetSecurityStrategy();
        Assert.IsNotNull (objectSecurityStrategy);
        Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
        DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
        Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
      }
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.AreSame (position.GetSecurityStrategy(), position.GetSecurityStrategy());
      }
    }

    [Test]
    public void GetSecurableType ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.AreSame (typeof (Position), position.GetSecurableType());
      }
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");
        IDomainObjectSecurityContextFactory factory = position;

        Assert.IsFalse (factory.IsInvalid);
        Assert.IsTrue (factory.IsNew);
        Assert.IsFalse (factory.IsDeleted);

        position.Delete();

        Assert.IsTrue (factory.IsInvalid);
      }
    }

    [Test]
    public void CreateSecurityContext ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");
        position.Delegation = Delegation.Enabled;

        ISecurityContext securityContext = ((ISecurityContextFactory) position).CreateSecurityContext();
        Assert.AreEqual (position.GetPublicDomainObjectType(), Type.GetType (securityContext.Class));
        Assert.IsNull (securityContext.Owner);
        Assert.IsNull (securityContext.OwnerGroup);
        Assert.IsNull (securityContext.OwnerTenant);
        Assert.That (securityContext.AbstractRoles, Is.Empty);
        Assert.AreEqual (1, securityContext.GetNumberOfStates ());
        Assert.AreEqual (EnumWrapper.Get (Delegation.Enabled), securityContext.GetState ("Delegation"));
      }
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("Position");

        Assert.IsNotEmpty (position.UniqueIdentifier);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position1 = testHelper.CreatePosition ("Position1");
        position1.UniqueIdentifier = "UID";

        Position position2 = testHelper.CreatePosition ("Position2");
        position2.UniqueIdentifier = "UID";

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }
  }
}
