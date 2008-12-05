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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchGroup : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _groupProperty;
    private Group _group;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new RolePropertiesSearchService ();
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClass (typeof (Role));
      _groupProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Group");
      Assert.That (_groupProperty, Is.Not.Null);

      _group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (_group, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_groupProperty), Is.True);
    }

    [Test]
    public void Search ()
    {
      User user = User.FindByUserName ("group0/user1");
      Assert.That (user, Is.Not.Null);
      Role role = _testHelper.CreateRole (user, _group, null);
      List<Group> expectedGroups = role.GetPossibleGroups (user.Tenant.ID);
      Assert.That (expectedGroups, Is.Not.Empty);

      IBusinessObject[] actualGroups = _searchService.Search (role, _groupProperty, null);

      Assert.That (actualGroups, Is.EquivalentTo (expectedGroups));
    }

    [Test]
    public void Search_WithRoleHasNoUser ()
    {
      Role role = _testHelper.CreateRole (null, _group, null);
      
      IBusinessObject[] actualGroups = _searchService.Search (role, _groupProperty, null);

      Assert.That (actualGroups, Is.Empty);
    }

    [Test]
    public void Search_WithUserHasNoTenant ()
    {
      User user = User.FindByUserName ("group0/user1");
      Assert.That (user, Is.Not.Null);
      user.Tenant = null;
      Role role = _testHelper.CreateRole (user, _group, null);

      IBusinessObject[] actualGroups = _searchService.Search (role, _groupProperty, null);

      Assert.That (actualGroups, Is.Empty);
    }
  }
}
