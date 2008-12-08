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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchUser : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _userProperty;
    private User _user;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new RolePropertiesSearchService();
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClass(typeof (Role));
      _userProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("User");
      Assert.That (_userProperty, Is.Not.Null);

      _user = User.FindByUserName ("group0/user1");
      Assert.That (_user, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_userProperty), Is.True);
    }

    [Test]
    public void Search ()
    {
      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      Role role = _testHelper.CreateRole (_user, group, null);
      DomainObjectCollection expectedGroups = User.FindByTenantID (group.Tenant.ID);
      Assert.That (expectedGroups, Is.Not.Empty);

      IBusinessObject[] actualUsers = _searchService.Search (role, _userProperty, null);

      Assert.That (actualUsers, Is.EquivalentTo (expectedGroups));
    }

    [Test]
    public void Search_WithRoleHasNoGroup ()
    {
      Role role = _testHelper.CreateRole (_user, null, null);

      IBusinessObject[] actualUsers = _searchService.Search (role, _userProperty, null);

      Assert.That (actualUsers, Is.Empty);
    }

    [Test]
    public void Search_WithGroupHasNoTenant ()
    {
      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      group.Tenant = null;
      Role role = _testHelper.CreateRole (_user, group, null);

      IBusinessObject[] actualUsers = _searchService.Search (role, _userProperty, null);

      Assert.That (actualUsers, Is.Empty);
    }
  }
}
