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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupPropertiesSearchServiceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _parentGroupProperty;
    private IBusinessObjectReferenceProperty _tenantProperty;
    private Group _group;

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
      _tenantProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Tenant");
      Assert.That (_parentGroupProperty, Is.Not.Null);

      _group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (_group, Is.Not.Null);
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
      Assert.That (_searchService.SupportsProperty (_tenantProperty), Is.False);
    }

    [Test]
    public void Search ()
    {
      List<Group> expectedParentGroups = _group.GetPossibleParentGroups (_group.Tenant.ID);
      Assert.That (expectedParentGroups, Is.Not.Empty);

      IBusinessObject[] actualParentGroups = _searchService.Search (_group, _parentGroupProperty, null);

      Assert.That (actualParentGroups, Is.EquivalentTo (expectedParentGroups));
    }

    [Test]
    public void Search_WithGroupHasNoTenant ()
    {
      _group.Tenant = null;

      IBusinessObject[] actualParentGroups = _searchService.Search (_group, _parentGroupProperty, null);

      Assert.That (actualParentGroups, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The property 'Tenant' is not supported by the 'Remotion.SecurityManager.Domain.OrganizationalStructure.GroupPropertiesSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {

      _searchService.Search (_group, _tenantProperty, null);
    }
  }
}
