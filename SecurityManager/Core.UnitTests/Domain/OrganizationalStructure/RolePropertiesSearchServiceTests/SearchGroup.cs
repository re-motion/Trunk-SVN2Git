/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchGroup : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _groupProperty;
    private Group _group;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new RolePropertiesSearchService ();
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (Role));
      _groupProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Group");
      Assert.That (_groupProperty, Is.Not.Null);

      _group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (_group, Is.Not.Null);
    }

    [Test]
    public void SupportsIdentity ()
    {
      Assert.That (_searchService.SupportsIdentity (_groupProperty), Is.True);
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
