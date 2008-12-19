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
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.ObjectBinding;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectSearchAllServiceTest : ClientTransactionBaseTest
  {
    private BindableDomainObjectSearchAllService _service;
    private SampleBindableDomainObject _persistedSampleObject1;
    private SampleBindableDomainObject _persistedSampleObject2;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        _persistedSampleObject1 = SampleBindableDomainObject.NewObject();
        _persistedSampleObject2 = SampleBindableDomainObject.NewObject();

        ClientTransaction.Current.Commit();
      }
    }

    public override void SetUp ()
    {
      base.SetUp ();
      _service = new BindableDomainObjectSearchAllService ();
      _persistedSampleObject1 = SampleBindableDomainObject.GetObject (_persistedSampleObject1.ID); // pull object into this transaction
      _persistedSampleObject2 = SampleBindableDomainObject.GetObject (_persistedSampleObject2.ID); // pull object into this transaction
    }

    [Test]
    public void SupportsProperty_True_SingleProperty ()
    {
      var property = (IBusinessObjectReferenceProperty) BindableDomainObjectProvider.GetBindableObjectClass (
          typeof (BindableDomainObjectWithProperties)).GetPropertyDefinition ("RequiredRelatedObjectProperty");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.True);
    }

    [Test]
    public void SupportsProperty_True_CollectionProperty ()
    {
      var property = (IBusinessObjectReferenceProperty) BindableDomainObjectProvider.GetBindableObjectClass (
          typeof (BindableDomainObjectWithProperties)).GetPropertyDefinition ("RequiredBidirectionalRelatedObjectsProperty");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.True);
    }

    [Test]
    public void SupportsProperty_False ()
    {
      var property = (IBusinessObjectReferenceProperty) BindableDomainObjectProvider.GetBindableObjectClass (
          typeof (BindableDomainObjectWithProperties)).GetPropertyDefinition ("ReferencePropertyNotInMapping");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "This service only supports queries for DomainObject types.\r\nParameter name: type")]
    public void GetAllObjects_ThrowsOnNonDomainObjects ()
    {
      _service.GetAllObjects (typeof (object));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "This service only supports queries for bindable DomainObject types, the " 
        + "given type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' is not a bindable type. Derive from BindableDomainObject or apply the "
        + "BindableDomainObjectAttribute.\r\nParameter name: type")]
    public void GetAllObjects_ThrowsOnNonBindableObjects ()
    {
      _service.GetAllObjects (typeof (Order));
    }

    [Test]
    public void GetAllObjects_WorksOnBindableDomainObjects ()
    {
      var result = _service.GetAllObjects (typeof (SampleBindableDomainObject));
      Assert.That (result, Is.EquivalentTo (new[] { _persistedSampleObject1, _persistedSampleObject2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The property 'ReferencePropertyNotInMapping' on type "
        + "'Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.BindableDomainObjectWithProperties, Remotion.Data.UnitTests' is not "
        + "supported by the BindableDomainObjectSearchAllService: The service only supports relation properties (ie. references to other DomainObject "
        + "instances).\r\nParameter name: property")]
    public void Search_ThrowsOnUnsupportedProperty ()
    {
      var property = (IBusinessObjectReferenceProperty) BindableDomainObjectProvider.GetBindableObjectClass (
          typeof (BindableDomainObjectWithProperties)).GetPropertyDefinition ("ReferencePropertyNotInMapping");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.False);

      _service.Search (null, property, null);
    }

    [Test]
    public void Search_SingleProperty ()
    {
      var property = (IBusinessObjectReferenceProperty) BindableDomainObjectProvider.GetBindableObjectClass (
          typeof (OppositeBidirectionalBindableDomainObject)).GetPropertyDefinition ("OppositeSampleObject");
      var result = _service.Search (null, property, null);
      Assert.That (result, Is.EquivalentTo (new[] { _persistedSampleObject1, _persistedSampleObject2 }));
    }

    [Test]
    public void Search_CollectionProperty ()
    {
      var property = (IBusinessObjectReferenceProperty) BindableDomainObjectProvider.GetBindableObjectClass (
          typeof (OppositeBidirectionalBindableDomainObject)).GetPropertyDefinition ("OppositeSampleObjects");
      var result = _service.Search (null, property, null);
      Assert.That (result, Is.EquivalentTo (new[] { _persistedSampleObject1, _persistedSampleObject2 }));
    }
  }
}