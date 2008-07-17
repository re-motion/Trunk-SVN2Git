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

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new UserPropertiesSearchService();
      IBusinessObjectClass groupClass = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (User));
      _owningGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("OwningGroup");
      _tenantProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Tenant");
      Assert.That (_owningGroupProperty, Is.Not.Null);

      _user = User.FindByUserName("group0/user1");
      Assert.That (_user, Is.Not.Null);
    }

    public override void TearDown ()
    {
      base.TearDown ();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SupportsIdentity ()
    {
      Assert.That (_searchService.SupportsIdentity (_owningGroupProperty), Is.True);
    }

    [Test]
    public void SupportsIdentity_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsIdentity (_tenantProperty), Is.False);
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
