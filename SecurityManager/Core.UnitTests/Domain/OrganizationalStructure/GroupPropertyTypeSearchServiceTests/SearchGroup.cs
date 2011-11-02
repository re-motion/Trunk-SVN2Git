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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupPropertyTypeSearchServiceTests
{
  [TestFixture]
  public class SearchGroup : GroupPropertyTypeSearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;
    private Group _group;
    private ObjectID _tenantID;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new GroupPropertyTypeSearchService();
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      _property = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Group");
      Assert.That (_property, Is.Not.Null);

      _group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (_group, Is.Not.Null);

      _tenantID = _group.Tenant.ID;
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
    public void Search_WithDisplayNameConstraint_FindNameContainingPrefix ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g.Name.Contains ("Group1")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, "Group1"));

      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindShortNameContainingPrefix ()
    {
      var expected = Group.FindByTenantID (_tenantID).Where (g => g.ShortName.Contains ("G1")).ToArray();
      Assert.That (expected.Length, Is.GreaterThan (1));

      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, null, "G1"));

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
      var actual = _searchService.Search (null, _property, new SecurityManagerSearchArguments (_tenantID, 1, "Group1"));

      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (((Group) actual[0]).Name, Is.StringContaining ("group1"));
    }
  }
}
