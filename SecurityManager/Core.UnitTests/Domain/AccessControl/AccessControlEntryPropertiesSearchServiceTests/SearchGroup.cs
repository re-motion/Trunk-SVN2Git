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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchGroup : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;
    private ObjectID _tenantID;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new AccessControlEntryPropertiesSearchService();
      IBusinessObjectClass aceClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (AccessControlEntry));
      _property = (IBusinessObjectReferenceProperty) aceClass.GetPropertyDefinition ("SpecificGroup");
      Assert.That (_property, Is.Not.Null);

      var tenant = Tenant.FindByUnqiueIdentifier ("UID: testTenant");
      Assert.That (tenant, Is.Not.Null);

      _tenantID = tenant.ID;
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_property), Is.True);
    }

    [Test]
    public void Search ()
    {
      var expected = Group.FindByTenantID (_tenantID).ToArray();
      Assert.That (expected, Is.Not.Empty);

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, null));

      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void Search_WithNameRestriction_FindNameContainingPrefix ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g.Name.Contains ("Group1")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, "Group1"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithNameRestriction_FindShortNameContainingPrefix ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g.ShortName.Contains ("G1")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, "G1"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }
  }
}