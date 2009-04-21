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
  public class UserPropertiesSearchServiceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _owningGroupProperty;
    private IBusinessObjectReferenceProperty _tenantProperty;
    private User _user;

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

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new UserPropertiesSearchService();
      IBusinessObjectClass groupClass = BindableObjectProvider.GetBindableObjectClass (typeof (User));
      _owningGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("OwningGroup");
      _tenantProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Tenant");
      Assert.That (_owningGroupProperty, Is.Not.Null);

      _user = User.FindByUserName("group0/user1");
      Assert.That (_user, Is.Not.Null);
    }

    public override void TestFixtureTearDown ()
    {
      base.TestFixtureTearDown ();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_owningGroupProperty), Is.True);
    }

    [Test]
    public void SupportsProperty_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_tenantProperty), Is.False);
    }

    [Test]
    public void Search ()
    {
      DomainObjectCollection expectedOwningGroups = Group.FindByTenantID (_user.Tenant.ID);
      Assert.That (expectedOwningGroups, Is.Not.Empty);

      IBusinessObject[] actualOwningGroups = _searchService.Search (_user, _owningGroupProperty, null);

      Assert.That (actualOwningGroups, Is.EquivalentTo (expectedOwningGroups));
    }

    [Test]
    public void Search_WithUserHasNoTenant ()
    {
      _user.Tenant = null;

      IBusinessObject[] actualOwningGroups = _searchService.Search (_user, _owningGroupProperty, null);

      Assert.That (actualOwningGroups, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The property 'Tenant' is not supported by the 'Remotion.SecurityManager.Domain.OrganizationalStructure.UserPropertiesSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {

      _searchService.Search (_user, _tenantProperty, null);
    }
  }
}
