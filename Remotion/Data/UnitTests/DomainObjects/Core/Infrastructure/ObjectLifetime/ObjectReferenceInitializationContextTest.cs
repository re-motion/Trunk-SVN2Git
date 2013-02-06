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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectReferenceInitializationContextTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerMock;
    private ClientTransaction _bindingClientTransaction;

    private ObjectReferenceInitializationContext _contextWithBindingTransaction;
    private ObjectReferenceInitializationContext _contextWithoutBindingTransaction;

    private DomainObject _boundObject;
    private DomainObject _unboundObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID = DomainObjectIDs.Order1;
      _enlistedDomainObjectManagerMock = MockRepository.GenerateStrictMock<IEnlistedDomainObjectManager> ();
      _bindingClientTransaction = ClientTransactionObjectMother.CreateBinding();

      _contextWithBindingTransaction = new ObjectReferenceInitializationContext (_objectID, _enlistedDomainObjectManagerMock, _bindingClientTransaction);
      _contextWithoutBindingTransaction = new ObjectReferenceInitializationContext (_objectID, _enlistedDomainObjectManagerMock, null);

      _boundObject = DomainObjectMother.GetObjectReference (_bindingClientTransaction, _objectID);
      _unboundObject = DomainObjectMother.CreateFakeObject (_objectID);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_contextWithBindingTransaction.ObjectID, Is.EqualTo (_objectID));
      Assert.That (_contextWithBindingTransaction.EnlistedDomainObjectManager, Is.SameAs (_enlistedDomainObjectManagerMock));
      Assert.That (_contextWithBindingTransaction.BindingTransaction, Is.SameAs (_bindingClientTransaction));
      Assert.That (_contextWithBindingTransaction.RegisteredObject, Is.Null);
    }

    [Test]
    public void Initialization_NullBindingTransaction ()
    {
      Assert.That (_contextWithoutBindingTransaction.BindingTransaction, Is.Null);
    }

    [Test]
    public void RegisterObject_Bound ()
    {
      _enlistedDomainObjectManagerMock
          .Expect (mock => mock.EnlistDomainObject (_boundObject))
          .Return (BooleanObjectMother.GetRandomBoolean());

      _contextWithBindingTransaction.RegisterObject (_boundObject);

      _enlistedDomainObjectManagerMock.VerifyAllExpectations();

      Assert.That (_contextWithBindingTransaction.RegisteredObject, Is.SameAs (_boundObject));
    }

    [Test]
    public void RegisterObject_Unbound ()
    {
      _enlistedDomainObjectManagerMock
          .Stub (stub => stub.EnlistDomainObject (_unboundObject))
          .Return (BooleanObjectMother.GetRandomBoolean ());

      _contextWithoutBindingTransaction.RegisterObject (_unboundObject);
    }

    [Test]
    public void RegisterObject_Twice ()
    {
      _enlistedDomainObjectManagerMock.Stub (mock => mock.EnlistDomainObject (_boundObject)).Return (BooleanObjectMother.GetRandomBoolean ());

      _contextWithBindingTransaction.RegisterObject (_boundObject);

      Assert.That (
          () => _contextWithBindingTransaction.RegisterObject (_boundObject),
          Throws.InvalidOperationException.With.Message.EqualTo ("Only one object can be registered using this context."));

      _enlistedDomainObjectManagerMock.VerifyAllExpectations ();

      Assert.That (_contextWithBindingTransaction.RegisteredObject, Is.SameAs (_boundObject));
    }

    [Test]
    public void RegisterObject_WrongID ()
    {
      var objectWithWrongID = DomainObjectMother.CreateFakeObject (DomainObjectIDs.Customer1);
      Assert.That (objectWithWrongID.ID, Is.Not.EqualTo (_objectID));

      Assert.That (
          () => _contextWithBindingTransaction.RegisterObject (objectWithWrongID),
          Throws.ArgumentException.With.Message.EqualTo ("The given DomainObject must have ID '" + _objectID + "'.\r\nParameter name: domainObject"));
    }

    [Test]
    public void RegisterObject_NotBound_ButShouldBe ()
    {
      Assert.That (_unboundObject.HasBindingTransaction, Is.False);
      Assert.That (_contextWithBindingTransaction.BindingTransaction, Is.Not.Null);

      Assert.That (
          () => _contextWithBindingTransaction.RegisterObject (_unboundObject),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given DomainObject must have BindingClientTransaction '" + _bindingClientTransaction + "'.\r\nParameter name: domainObject"));
    }

    [Test]
    public void RegisterObject_Bound_ButShouldNotBe ()
    {
      Assert.That (_boundObject.HasBindingTransaction, Is.True);
      Assert.That (_contextWithoutBindingTransaction.BindingTransaction, Is.Null);

      Assert.That (
          () => _contextWithoutBindingTransaction.RegisterObject (_boundObject),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given DomainObject must not have a BindingClientTransaction.\r\nParameter name: domainObject"));
    }

    [Test]
    public void RegisterObject_BoundToWrongTransaction ()
    {
      var differentlyBoundObject = DomainObjectMother.GetObjectReference (ClientTransactionObjectMother.CreateBinding(), _objectID);
      Assert.That (differentlyBoundObject.GetBindingTransaction(), Is.Not.SameAs (_contextWithBindingTransaction.BindingTransaction));

      Assert.That (
          () => _contextWithBindingTransaction.RegisterObject (differentlyBoundObject),
          Throws.ArgumentException.With.Message.EqualTo (
              "The given DomainObject must have BindingClientTransaction '" + _bindingClientTransaction + "'.\r\nParameter name: domainObject"));
    }
  }
}