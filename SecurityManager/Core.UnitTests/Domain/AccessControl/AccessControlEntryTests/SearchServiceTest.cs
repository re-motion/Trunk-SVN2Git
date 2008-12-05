// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class SearchSerivceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private AccessControlTestHelper _testHelper;
    private IBusinessObjectClass _aceClass;
    private AccessControlEntry _ace;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute> ().AddService (typeof (AccessControlEntryPropertiesSearchService), new AccessControlEntryPropertiesSearchService ());
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute> ().AddService (typeof (ISearchAvailableObjectsService), MockRepository.GenerateStub<ISearchAvailableObjectsService>());

      _dbFixtures = new DatabaseFixtures ();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction ());
    }

    public override void SetUp ()
    {
      base.SetUp();
      
      _aceClass = BindableObjectProvider.GetBindableObjectClass (typeof (AccessControlEntry));
    
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
      
      _ace = AccessControlEntry.NewObject ();
    }

    public override void TestFixtureTearDown ()
    {
      base.TestFixtureTearDown ();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SearchSpecificTenants ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificTenant");
      Assert.That (property, Is.Not.Null);

      ObjectList<Tenant> expected = Tenant.FindAll();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificGroups ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificGroup");
      Assert.That (property, Is.Not.Null);

      var tenant = Tenant.FindByUnqiueIdentifier ("UID: testTenant");
      Assert.That (tenant, Is.Not.Null);

      ObjectList<Group> expected = Group.FindByTenantID (tenant.ID);
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, new DefaultSearchArguments (tenant.ID.ToString()));
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificGroupType ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificGroupType");
      Assert.That (property, Is.Not.Null);

      ObjectList<GroupType> expected = GroupType.FindAll ();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificUsers ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificUser");
      Assert.That (property, Is.Not.Null);

      var tenant = Tenant.FindByUnqiueIdentifier ("UID: testTenant");
      Assert.That (tenant, Is.Not.Null);

      ObjectList<User> expected = User.FindByTenantID (tenant.ID);
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, new DefaultSearchArguments (tenant.ID.ToString ()));
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificPositions ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificPosition");
      Assert.That (property, Is.Not.Null);

      ObjectList<Position> expected = Position.FindAll ();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificAbstractRoles ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificAbstractRole");
      Assert.That (property, Is.Not.Null);

      ObjectList<AbstractRoleDefinition> expected = AbstractRoleDefinition.FindAll();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }
  }
}
