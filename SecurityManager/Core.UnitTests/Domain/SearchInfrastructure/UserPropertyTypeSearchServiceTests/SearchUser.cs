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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.UserPropertyTypeSearchServiceTests
{
  [TestFixture]
  public class SearchUser : SearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;
    private ObjectID _tenantID;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new UserPropertyTypeSearchService();
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      _property = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("User");
      Assert.That (_property, Is.Not.Null);

      var user = User.FindByUserName ("group0/user1");
      Assert.That (user, Is.Not.Null);

      _tenantID = user.Tenant.ID;
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_property), Is.True);
    }

    [Test]
    public void Search ()
    {
      var expected = User.FindByTenantID (_tenantID);
      Assert.That (expected, Is.Not.Empty);

      IBusinessObject[] actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, null));

      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindLastNameContainingPrefix ()
    {
      var expected = User.FindByTenantID (_tenantID).Where (u => u.LastName.Contains ("user")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, "user"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindFirstNameContainingPrefix ()
    {
      var expected = User.FindByTenantID (_tenantID).Where (u => u.FirstName.Contains ("est")).ToArray();
      Assert.That (expected, Is.Not.Empty);

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, "est"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithResultSizeConstraint ()
    {
      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, 3, null));

      Assert.That (actual.Length, Is.EqualTo (3));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_AndResultSizeConstrant ()
    {
      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, 1, "user")).ToArray();

      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (((User) actual[0]).LastName, Is.StringContaining ("user"));
    }
  }
}