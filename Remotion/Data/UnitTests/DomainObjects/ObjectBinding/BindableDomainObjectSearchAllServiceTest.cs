// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

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
      var property = GetBusinessObjectProperty (typeof (BindableDomainObjectWithProperties), "RequiredRelatedObjectProperty");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.True);
    }

    [Test]
    public void SupportsProperty_True_CollectionProperty ()
    {
      var property = GetBusinessObjectProperty (typeof (BindableDomainObjectWithProperties), "RequiredBidirectionalRelatedObjectsProperty");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.True);
    }

    [Test]
    public void SupportsProperty_False ()
    {
      var property = GetBusinessObjectProperty (typeof (BindableDomainObjectWithProperties), "ReferencePropertyNotInMapping");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "This service only supports queries for DomainObject types.\r\nParameter name: type")]
    public void GetAllObjects_ThrowsOnNonDomainObjects ()
    {
      _service.GetAllObjects (ClientTransaction.Current, typeof (object));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "This service only supports queries for bindable DomainObject types, the " 
        + "given type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' is not a bindable type. Derive from BindableDomainObject or apply the "
        + "BindableDomainObjectAttribute.\r\nParameter name: type")]
    public void GetAllObjects_ThrowsOnNonBindableObjects ()
    {
      _service.GetAllObjects (ClientTransaction.Current, typeof (Order));
    }

    [Test]
    public void GetAllObjects_WorksOnBindableDomainObjects ()
    {
      var result = _service.GetAllObjects (ClientTransaction.Current, typeof (SampleBindableDomainObject));
      Assert.That (result, Is.EquivalentTo (new[] { _persistedSampleObject1, _persistedSampleObject2 }));
    }

    [Test]
    public void GetAllObjects_DifferentTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      var result = _service.GetAllObjects (transaction, typeof (SampleBindableDomainObject));
      Assert.That (transaction.IsEnlisted ((DomainObject) result[0]), Is.True);
      Assert.That (ClientTransaction.Current.IsEnlisted ((DomainObject) result[0]), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The property 'ReferencePropertyNotInMapping' on type "
        + "'Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain.BindableDomainObjectWithProperties, Remotion.Data.UnitTests' is not "
        + "supported by the BindableDomainObjectSearchAllService: The service only supports relation properties (ie. references to other DomainObject "
        + "instances).\r\nParameter name: property")]
    public void Search_ThrowsOnUnsupportedProperty ()
    {
      var property = GetBusinessObjectProperty (typeof (BindableDomainObjectWithProperties), "ReferencePropertyNotInMapping");
      Assert.That (property, Is.Not.Null);
      Assert.That (_service.SupportsProperty (property), Is.False);

      _service.Search (null, property, null);
    }

    [Test]
    public void Search_SingleProperty ()
    {
      var property = GetBusinessObjectProperty (typeof (OppositeBidirectionalBindableDomainObject), "OppositeSampleObject");
      var result = _service.Search (null, property, null);
      Assert.That (result, Is.EquivalentTo (new[] { _persistedSampleObject1, _persistedSampleObject2 }));
    }

    [Test]
    public void Search_CollectionProperty ()
    {
      var property = GetBusinessObjectProperty (typeof (OppositeBidirectionalBindableDomainObject), "OppositeSampleObjects");
      var result = _service.Search (null, property, null);
      Assert.That (result, Is.EquivalentTo (new[] { _persistedSampleObject1, _persistedSampleObject2 }));
    }

    [Test]
    public void Search_UsesCurrentTransaction_WithNullObject ()
    {
      var property = GetBusinessObjectProperty (typeof (OppositeBidirectionalBindableDomainObject), "OppositeSampleObject");
      var result = _service.Search (null, property, null);
      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (((DomainObject) result[0]).HasBindingTransaction, Is.False);
      Assert.That (ClientTransactionMock.Current.IsEnlisted ((DomainObject) result[0]), Is.True);
    }

    [Test]
    public void Search_UsesCurrentTransaction_WithNonDomainObject ()
    {
      var property = GetBusinessObjectProperty (typeof (BindableNonDomainObjectReferencingDomainObject), "OppositeSampleObject");
      var result = _service.Search (new BindableNonDomainObjectReferencingDomainObject(), property, null);
      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (((DomainObject) result[0]).HasBindingTransaction, Is.False);
      Assert.That (ClientTransactionMock.Current.IsEnlisted (((DomainObject) result[0])), Is.True);
    }

    [Test]
    public void Search_UsesCurrentTransaction_WithUnboundObject ()
    {
      var referencingObject = OppositeBidirectionalBindableDomainObject.NewObject();

      var property = GetBusinessObjectProperty (typeof (OppositeBidirectionalBindableDomainObject), "OppositeSampleObject");
      var result = _service.Search (referencingObject, property, null);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (((DomainObject) result[0]).HasBindingTransaction, Is.False);
      Assert.That (ClientTransactionMock.Current.IsEnlisted ((DomainObject) result[0]), Is.True);
    }

    [Test]
    public void Search_UsesBindingTransaction_WithBoundObject ()
    {
      OppositeBidirectionalBindableDomainObject referencingObject;
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        referencingObject = OppositeBidirectionalBindableDomainObject.NewObject();
      }

      var property = GetBusinessObjectProperty (typeof (OppositeBidirectionalBindableDomainObject), "OppositeSampleObject");
      var result = _service.Search (referencingObject, property, null);
      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (((DomainObject) result[0]).GetBindingTransaction(), Is.SameAs (bindingTransaction));
      Assert.That (((DomainObject) result[1]).GetBindingTransaction(), Is.SameAs (bindingTransaction));
      Assert.That (ClientTransactionMock.Current.IsEnlisted ((DomainObject) result[0]), Is.False);
      Assert.That (ClientTransactionMock.Current.IsEnlisted ((DomainObject) result[1]), Is.False);
      Assert.That (bindingTransaction.IsEnlisted ((DomainObject) result[0]), Is.True);
      Assert.That (bindingTransaction.IsEnlisted ((DomainObject) result[1]), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread or " 
        + "the referencing object.")]
    public void Search_NoCurrentTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        var property = GetBusinessObjectProperty (typeof (OppositeBidirectionalBindableDomainObject), "OppositeSampleObject");
        _service.Search (null, property, null);
      }
    }

    private IBusinessObjectReferenceProperty GetBusinessObjectProperty (Type bindableObjectType, string propertyName)
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (bindableObjectType);
      var bindableObjectClass = provider.GetBindableObjectClass (bindableObjectType);
      return (IBusinessObjectReferenceProperty) bindableObjectClass.GetPropertyDefinition (propertyName);
    }
  }
}
