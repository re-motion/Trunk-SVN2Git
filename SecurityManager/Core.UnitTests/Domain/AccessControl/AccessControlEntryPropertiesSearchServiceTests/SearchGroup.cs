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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchGroup : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;

    public override void SetUp ()
    {
      base.SetUp();
      
      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new AccessControlEntryPropertiesSearchService();
      IBusinessObjectClass aceClass = BindableObjectProvider.GetBindableObjectClass (typeof (AccessControlEntry));
      _property = (IBusinessObjectReferenceProperty) aceClass.GetPropertyDefinition ("SpecificGroup");
      Assert.That (_property, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_property), Is.True);
    }

    [Test]
    public void Search ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      var tenant = Tenant.FindByUnqiueIdentifier ("UID: testTenant");
      Assert.That (tenant, Is.Not.Null);

      ObjectList<Group> expected = Group.FindByTenantID (tenant.ID);
      Assert.That (expected, Is.Not.Empty);

      IBusinessObject[] actual = _searchService.Search (ace, _property, new DefaultSearchArguments (tenant.ID.ToString()));

      Assert.That (actual, Is.EqualTo (expected));
    }
  }
}