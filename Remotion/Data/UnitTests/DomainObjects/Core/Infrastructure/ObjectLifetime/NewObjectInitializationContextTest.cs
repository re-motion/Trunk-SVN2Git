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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class NewObjectInitializationContextTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerMock;
    private IDataManager _dataManagerMock;
    private ClientTransaction _bindingClientTransaction;

    private NewObjectInitializationContext _contextWithBindingTransaction;
    private NewObjectInitializationContext _contextWithoutBindingTransaction;

    private DomainObject _boundObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID = DomainObjectIDs.Order1;
      _enlistedDomainObjectManagerMock = MockRepository.GenerateStrictMock<IEnlistedDomainObjectManager> ();
      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager> ();
      _bindingClientTransaction = ClientTransactionObjectMother.CreateBinding();

      _contextWithBindingTransaction = new NewObjectInitializationContext (
          _objectID, _enlistedDomainObjectManagerMock, _dataManagerMock, _bindingClientTransaction);
      _contextWithoutBindingTransaction = new NewObjectInitializationContext (_objectID, _enlistedDomainObjectManagerMock, _dataManagerMock, null);

      _boundObject = DomainObjectMother.GetObjectReference (_bindingClientTransaction, _objectID);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_contextWithBindingTransaction.ObjectID, Is.EqualTo (_objectID));
      Assert.That (_contextWithBindingTransaction.EnlistedDomainObjectManager, Is.SameAs (_enlistedDomainObjectManagerMock));
      Assert.That (_contextWithBindingTransaction.DataManager, Is.SameAs (_dataManagerMock));
      Assert.That (_contextWithBindingTransaction.BindingTransaction, Is.SameAs (_bindingClientTransaction));
      Assert.That (_contextWithBindingTransaction.RegisteredObject, Is.Null);
    }

    [Test]
    public void Initialization_NullBindingTransaction ()
    {
      Assert.That (_contextWithoutBindingTransaction.BindingTransaction, Is.Null);
    }

    [Test]
    public void RegisterObject ()
    {
      var counter = new OrderedExpectationCounter();
      _enlistedDomainObjectManagerMock
          .Expect (mock => mock.EnlistDomainObject (_boundObject))
          .Return (BooleanObjectMother.GetRandomBoolean())
          .Ordered (counter);
      StubEmptyDataContainersCollection (_dataManagerMock);
      _dataManagerMock
          .Expect (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything))
          .WhenCalled (mi =>
          {
            var dc = (DataContainer) mi.Arguments[0];
            Assert.That (dc.ID, Is.EqualTo (_objectID));
            Assert.That (dc.DomainObject, Is.SameAs (_boundObject));
          })
          .Ordered (counter);

      _contextWithBindingTransaction.RegisterObject (_boundObject);

      _enlistedDomainObjectManagerMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();

      Assert.That (_contextWithBindingTransaction.RegisteredObject, Is.SameAs (_boundObject));
    }
    
    [Test]
    public void RegisterObject_WithError ()
    {
      var objectWithWrongID = DomainObjectMother.CreateFakeObject (DomainObjectIDs.Customer1);
      Assert.That (objectWithWrongID.ID, Is.Not.EqualTo (_objectID));

      Assert.That (
          () => _contextWithBindingTransaction.RegisterObject (objectWithWrongID),
          Throws.ArgumentException.With.Message.EqualTo ("The given DomainObject must have ID '" + _objectID + "'.\r\nParameter name: domainObject"));

      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    private void StubEmptyDataContainersCollection (IDataManager dataManagerMock)
    {
      dataManagerMock.Stub (stub => stub.DataContainers).Return (MockRepository.GenerateStub<IDataContainerMapReadOnlyView> ());
    }
  }
}