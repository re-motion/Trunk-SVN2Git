// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
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
        var positions = Position.FindAll();

        Assert.That (positions.Count(), Is.EqualTo (3));
      }
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);
        User user = User.FindByTenant (tenant.GetHandle()).First();
        Role role = user.Roles[0];
        Position position = role.Position;
        AccessControlEntry ace = testHelper.CreateAceWithPosition (position);
        ClientTransaction.Current.Commit();

        position.Delete();

        ClientTransaction.Current.Commit();

        Assert.That (ace.IsInvalid, Is.True);
      }
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);
        User user = User.FindByTenant (tenant.GetHandle()).First();
        Role role = user.Roles[0];
        Position position = role.Position;
        position.Delete ();

        ClientTransaction.Current.Commit ();

        Assert.That (role.IsInvalid, Is.True);
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

        Assert.That (concretePosition.IsInvalid, Is.True);
      }
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");

        Assert.That (position.DisplayName, Is.EqualTo ("PositionName"));
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
        Assert.That (objectSecurityStrategy, Is.Not.Null);
        Assert.IsInstanceOf (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
        DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
        Assert.That (domainObjectSecurityStrategy.RequiredSecurityForStates, Is.EqualTo (RequiredSecurityForStates.None));
      }
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.That (position.GetSecurityStrategy(), Is.SameAs (position.GetSecurityStrategy()));
      }
    }

    [Test]
    public void GetSecurableType ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.That (position.GetSecurableType(), Is.SameAs (typeof (Position)));
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

        Assert.That (factory.IsInvalid, Is.False);
        Assert.That (factory.IsNew, Is.True);
        Assert.That (factory.IsDeleted, Is.False);

        position.Delete();

        Assert.That (factory.IsInvalid, Is.True);
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
        Assert.That (Type.GetType (securityContext.Class), Is.EqualTo (position.GetPublicDomainObjectType()));
        Assert.That (securityContext.Owner, Is.Null);
        Assert.That (securityContext.OwnerGroup, Is.Null);
        Assert.That (securityContext.OwnerTenant, Is.Null);
        Assert.That (securityContext.AbstractRoles, Is.Empty);
        Assert.That (securityContext.GetNumberOfStates (), Is.EqualTo (1));
        Assert.That (securityContext.GetState ("Delegation"), Is.EqualTo (EnumWrapper.Get (Delegation.Enabled)));
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
