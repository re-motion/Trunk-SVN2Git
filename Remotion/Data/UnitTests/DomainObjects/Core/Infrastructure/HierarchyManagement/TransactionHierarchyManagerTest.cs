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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class TransactionHierarchyManagerTest : StandardMappingTest
  {
    private ClientTransaction _thisTransaction;
    private ClientTransactionEventSinkWithMock _thisEventSinkWithStrictMock;

    private ClientTransaction _parentTransaction;
    private ITransactionHierarchyManager _parentHierarchyManagerStrictMock;
    private ClientTransactionEventSinkWithMock _parentEventSinkWithStrictMock;

    private TransactionHierarchyManager _manager;
    private TransactionHierarchyManager _managerWithNullParent;

    public override void SetUp ()
    {
      base.SetUp();

      _thisTransaction = ClientTransactionObjectMother.Create ();
      _thisEventSinkWithStrictMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock();
      _parentTransaction = ClientTransactionObjectMother.Create ();
      _parentHierarchyManagerStrictMock = MockRepository.GenerateStrictMock<ITransactionHierarchyManager>();
      _parentEventSinkWithStrictMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock ();

      _manager = new TransactionHierarchyManager (
          _thisTransaction, _thisEventSinkWithStrictMock, _parentTransaction, _parentHierarchyManagerStrictMock, _parentEventSinkWithStrictMock);
      _managerWithNullParent = new TransactionHierarchyManager (_thisTransaction, _thisEventSinkWithStrictMock, null, null, null);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_manager.ThisTransaction, Is.SameAs (_thisTransaction));
      Assert.That (_manager.ThisEventSink, Is.SameAs (_thisEventSinkWithStrictMock));
      Assert.That (_manager.ParentTransaction, Is.SameAs (_parentTransaction));
      Assert.That (_manager.ParentHierarchyManager, Is.SameAs (_parentHierarchyManagerStrictMock));
      Assert.That (_manager.ParentEventSink, Is.SameAs (_parentEventSinkWithStrictMock));
      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void Initialization_NullParent ()
    {
      Assert.That (_managerWithNullParent.ThisTransaction, Is.SameAs (_thisTransaction));
      Assert.That (_managerWithNullParent.ThisEventSink, Is.SameAs (_thisEventSinkWithStrictMock));
      Assert.That (_managerWithNullParent.ParentTransaction, Is.Null);
      Assert.That (_managerWithNullParent.ParentHierarchyManager, Is.Null);
      Assert.That (_managerWithNullParent.ParentEventSink, Is.Null);
      Assert.That (_managerWithNullParent.IsActive, Is.True);
      Assert.That (_managerWithNullParent.SubTransaction, Is.Null);
    }

    [Test]
    public void InstallListeners ()
    {
      var eventBrokerMock = MockRepository.GenerateStrictMock<IClientTransactionEventBroker>();
      eventBrokerMock.Expect (mock => mock.AddListener (Arg<InactiveClientTransactionListener>.Is.TypeOf));
      eventBrokerMock.Expect (mock => mock.AddListener (Arg<NewObjectHierarchyInvalidationClientTransactionListener>.Is.TypeOf));

      _manager.InstallListeners (eventBrokerMock);
    }

    [Test]
    public void OnBeforeTransactionInitialize ()
    {
      _parentEventSinkWithStrictMock.ExpectMock (mock => mock.SubTransactionInitialize (_parentEventSinkWithStrictMock.ClientTransaction, _thisTransaction));
      ClientTransactionTestHelper.SetIsActive (_parentTransaction, false); // required by assertion in InactiveClientTransactionListener

      _manager.OnBeforeTransactionInitialize();

      _parentEventSinkWithStrictMock.VerifyMock();
    }

    [Test]
    public void OnBeforeTransactionInitialize_NullParent ()
    {
      Assert.That (() => _managerWithNullParent.OnBeforeTransactionInitialize(), Throws.Nothing);
    }

    [Test]
    public void OnTransactionDiscard ()
    {
      _parentHierarchyManagerStrictMock.Expect (mock => mock.RemoveSubTransaction());

      _manager.OnTransactionDiscard ();

      _parentHierarchyManagerStrictMock.VerifyAllExpectations ();
    }

    [Test]
    public void OnTransactionDiscard_NullParent ()
    {
      Assert.That (() => _managerWithNullParent.OnTransactionDiscard (), Throws.Nothing);
    }

    [Test]
    public void OnBeforeObjectRegistration_WithoutParent ()
    {
      Assert.That (_managerWithNullParent.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);

      _managerWithNullParent.OnBeforeObjectRegistration (Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));

      Assert.That (
          _managerWithNullParent.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs, 
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));

      _managerWithNullParent.OnBeforeObjectRegistration (Array.AsReadOnly (new[] { DomainObjectIDs.Order3 }));

      Assert.That (
          _managerWithNullParent.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 }));
    }

    [Test]
    public void OnBeforeObjectRegistration_WithParent ()
    {
      Assert.That (_manager.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);

      _parentHierarchyManagerStrictMock
          .Expect (mock => mock.OnBeforeSubTransactionObjectRegistration (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }))
          .WhenCalled (mi => Assert.That (_manager.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty));

      _manager.OnBeforeObjectRegistration (Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));

      _parentHierarchyManagerStrictMock.VerifyAllExpectations();
      Assert.That (
          _manager.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));
    }

    [Test]
    public void OnAfterObjectRegistration ()
    {
      _managerWithNullParent.OnBeforeObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 }));
      Assert.That (
          _managerWithNullParent.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 }));

      _managerWithNullParent.OnAfterObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));

      Assert.That (
          _managerWithNullParent.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order3 }));

      _managerWithNullParent.OnAfterObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order3 }));

      Assert.That (_managerWithNullParent.InactiveClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);
    }

    [Test]
    public void OnBeforeSubTransactionObjectRegistration_NoConflicts ()
    {
      Assert.That (
          () =>_manager.OnBeforeSubTransactionObjectRegistration (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }),
          Throws.Nothing);
    }

    [Test]
    public void OnBeforeSubTransactionObjectRegistration_Conflicts ()
    {
      _managerWithNullParent.OnBeforeObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 }));

      Assert.That (
          () => _managerWithNullParent.OnBeforeSubTransactionObjectRegistration (
              new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order4 }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'."));
    }

    [Test]
    public void CreateSubTransaction ()
    {
      bool subTransactionCreatingCalled = false;
      _thisEventSinkWithStrictMock
          .ExpectMock (mock => mock.SubTransactionCreating (_thisEventSinkWithStrictMock.ClientTransaction))
          .WhenCalled (mi =>
          {
            Assert.That (_manager.IsActive, Is.True);
            subTransactionCreatingCalled = true;
          });

      ClientTransaction fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent (_thisTransaction);
      Func<ClientTransaction, ClientTransaction> factory = tx =>
      {
        Assert.That (tx, Is.SameAs (_thisTransaction));
        Assert.That (subTransactionCreatingCalled, Is.True);
        Assert.That (_manager.IsActive, Is.False, "IsActive needs to be set before the factory is called.");
        ClientTransactionTestHelper.SetIsActive (_thisTransaction, false); // required by assertion in InactiveClientTransactionListener
        return fakeSubTransaction;
      };

      _thisEventSinkWithStrictMock.ExpectMock (mock => mock.SubTransactionCreated (_thisEventSinkWithStrictMock.ClientTransaction, fakeSubTransaction));

      var result = _manager.CreateSubTransaction (factory);

      Assert.That (result, Is.Not.Null.And.SameAs (fakeSubTransaction));
      Assert.That (_manager.SubTransaction, Is.SameAs (fakeSubTransaction));
      Assert.That (_manager.IsActive, Is.False);
    }

    [Test]
    public void CreateSubTransaction_InvalidFactory ()
    {
      _thisEventSinkWithStrictMock.ExpectMock (mock => mock.SubTransactionCreating (_thisEventSinkWithStrictMock.ClientTransaction));

      var fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent (null);
      Func<ClientTransaction, ClientTransaction> factory = tx => fakeSubTransaction;

      Assert.That (
          () => _manager.CreateSubTransaction (factory), 
          Throws.InvalidOperationException.With.Message.EqualTo ("The given factory did not create a sub-transaction for this transaction."));

      _thisEventSinkWithStrictMock.AssertWasNotCalledMock (
          mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<ClientTransaction>.Is.Anything));
      _thisEventSinkWithStrictMock.VerifyMock ();

      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.IsActive, Is.True);
    }

    [Test]
    public void CreateSubTransaction_ThrowingFactory ()
    {
      _thisEventSinkWithStrictMock.ExpectMock (mock => mock.SubTransactionCreating (_thisEventSinkWithStrictMock.ClientTransaction));

      var exception = new Exception();
      Func<ClientTransaction, ClientTransaction> factory = tx => { throw exception; };

      Assert.That (
          () => _manager.CreateSubTransaction (factory),
          Throws.Exception.SameAs (exception));

      _thisEventSinkWithStrictMock.AssertWasNotCalledMock (
          mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<ClientTransaction>.Is.Anything));
      _thisEventSinkWithStrictMock.VerifyMock ();

      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.IsActive, Is.True);
    }

    [Test]
    public void CreateSubTransaction_BeginEventAbortsOperation ()
    {
      var exception = new Exception ();
      _thisEventSinkWithStrictMock.ExpectMock (mock => mock.SubTransactionCreating (_thisEventSinkWithStrictMock.ClientTransaction)).Throw (exception);

      Func<ClientTransaction, ClientTransaction> factory = tx => { Assert.Fail ("Must not be called."); return null; };

      Assert.That (
          () => _manager.CreateSubTransaction (factory),
          Throws.Exception.SameAs (exception));

      _thisEventSinkWithStrictMock.AssertWasNotCalledMock (
          mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<ClientTransaction>.Is.Anything));
      _thisEventSinkWithStrictMock.VerifyMock ();

      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.IsActive, Is.True);
    }

    [Test]
    public void RemoveSubTransaction_NoSubtransaction ()
    {
      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);

      _manager.RemoveSubTransaction();

      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void RemoveSubTransaction_WithSubtransaction ()
    {
      FakeManagerWithSubtransaction (_manager);

      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      _manager.RemoveSubTransaction ();

      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void Unlock_Active ()
    {
      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);

      Assert.That (
          () => _manager.Unlock(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              _thisTransaction + " cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed "
              + "while its parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used."));

      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void Unlock_Inactive ()
    {
      FakeManagerWithSubtransaction (_manager);
      
      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      var result = _manager.Unlock();

      Assert.That (result, Is.Not.Null);
      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose();

      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose ();

      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);
    }

    [Test]
    public void UnlockIfRequired_Active ()
    {
      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);

      var result = _manager.UnlockIfRequired();
      Assert.That (result, Is.Null);

      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void UnlockIfRequired_Inactive ()
    {
      FakeManagerWithSubtransaction (_manager);

      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      var result = _manager.UnlockIfRequired ();

      Assert.That (result, Is.Not.Null);
      Assert.That (_manager.IsActive, Is.True);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose ();

      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose ();

      Assert.That (_manager.IsActive, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);
    }

    [Test]
    public void Serializable ()
    {
      var instance = new TransactionHierarchyManager (
          ClientTransactionObjectMother.Create(),
          new SerializableClientTransactionEventSinkFake(),
          ClientTransactionObjectMother.Create (), 
          new SerializableTransactionHierarchyManagerFake(),
          new SerializableClientTransactionEventSinkFake());
      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.IsActive, Is.True);
      Assert.That (deserializedInstance.SubTransaction, Is.Null);
      Assert.That (deserializedInstance.ThisTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.ParentTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.ParentHierarchyManager, Is.Not.Null);
    }

    private void FakeManagerWithSubtransaction (TransactionHierarchyManager transactionHierarchyManager)
    {
      TransactionHierarchyManagerTestHelper.SetIsActive (transactionHierarchyManager, false);
      var fakeSubTransaction = ClientTransactionObjectMother.Create ();
      TransactionHierarchyManagerTestHelper.SetSubtransaction (transactionHierarchyManager, fakeSubTransaction);
    }
  }
}