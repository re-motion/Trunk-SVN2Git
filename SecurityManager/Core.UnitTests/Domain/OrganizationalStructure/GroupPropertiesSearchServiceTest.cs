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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupPropertiesSearchServiceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _parentGroupProperty;
    private Group _group;
    private ObjectID _tenantID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      
      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new GroupPropertiesSearchService();
      IBusinessObjectClass groupClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Group));
      _parentGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Parent");
      Assert.That (_parentGroupProperty, Is.Not.Null);

      _group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (_group, Is.Not.Null);

      _tenantID = _group.Tenant.ID;
    }

    public override void TestFixtureTearDown ()
    {
      base.TestFixtureTearDown ();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_parentGroupProperty), Is.True);
    }

    [Test]
    public void SupportsProperty_WithInvalidProperty ()
    {
      IBusinessObjectClass groupClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Group));
      var otherProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Tenant");
      Assert.That (_searchService.SupportsProperty (otherProperty), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The property 'Tenant' is not supported by the 'Remotion.SecurityManager.Domain.OrganizationalStructure.GroupPropertiesSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {
      IBusinessObjectClass groupClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Group));
      var otherProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Tenant");
      _searchService.Search (_group, otherProperty, null);
    }

    [Test]
    public void Search ()
    {
      var expected = Group.FindByTenantID (_tenantID).ToArray();
      Assert.That (expected, Is.Not.Empty);

      var actual = _searchService.Search (null, _parentGroupProperty, new SecurityManagerSearchArguments (_tenantID, null, null));

      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void Search_ExcludeReferencingGroup ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g != _group).ToArray();
      Assert.That (expected, Is.Not.Empty);

      var actual = _searchService.Search (_group, _parentGroupProperty, new SecurityManagerSearchArguments (_tenantID, null, null));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindNameContainingPrefix ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g.Name.Contains ("Group1")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _parentGroupProperty, new SecurityManagerSearchArguments (_tenantID, null, "Group1"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindShortNameContainingPrefix ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g.ShortName.Contains ("G1")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _parentGroupProperty, new SecurityManagerSearchArguments (_tenantID, null, "G1"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithResultSizeConstraint ()
    {
      var actual = _searchService.Search (null, _parentGroupProperty, new SecurityManagerSearchArguments (_tenantID, 3, null));

      Assert.That (actual.Length, Is.EqualTo (3));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_AndResultSizeConstrant ()
    {
      var actual = _searchService.Search (null, _parentGroupProperty, new SecurityManagerSearchArguments (_tenantID, 1, "Group1"));

      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (((Group) actual[0]).Name, Is.StringContaining ("group1"));
    }
  }
}
