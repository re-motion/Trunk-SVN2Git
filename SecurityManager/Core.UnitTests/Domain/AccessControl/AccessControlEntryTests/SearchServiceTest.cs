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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class SearchSerivceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private AccessControlTestHelper _testHelper;
    private IBusinessObjectClass _aceClass;
    private AccessControlEntry _ace;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute> ().AddService (typeof (AccessControlEntryPropertiesSearchService), new AccessControlEntryPropertiesSearchService ());
  
      _dbFixtures = new DatabaseFixtures ();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction ());
    }

    public override void SetUp ()
    {
      base.SetUp();
      
      _aceClass = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (AccessControlEntry));
    
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
      
      _ace = AccessControlEntry.NewObject ();
    }

    public override void TestFixtureTearDown ()
    {
      base.TestFixtureTearDown ();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SearchSpecificTenants ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificTenant");
      Assert.That (property, Is.Not.Null);

      ObjectList<Tenant> expected = Tenant.FindAll();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificPositions ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificPosition");
      Assert.That (property, Is.Not.Null);

      ObjectList<Position> expected = Position.FindAll ();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificAbstractRoles ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificAbstractRole");
      Assert.That (property, Is.Not.Null);

      ObjectList<AbstractRoleDefinition> expected = AbstractRoleDefinition.FindAll();
      Assert.That (expected, Is.Not.Empty);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, null);
      Assert.That (actual, Is.EquivalentTo (expected));
    }
  }
}
