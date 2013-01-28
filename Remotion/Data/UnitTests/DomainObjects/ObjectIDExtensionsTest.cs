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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects
{
  [TestFixture]
  public class ObjectIDExtensionsTest : StandardMappingTest
  {
    private IObjectID<Order> _orderID1;
    private IObjectID<Order> _orderID2;
    private IObjectID<Order> _notFoundOrderID;
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderID1 = (IObjectID<Order>) DomainObjectIDs.Order1;
      _orderID2 = (IObjectID<Order>) DomainObjectIDs.Order2;
      _notFoundOrderID = ObjectID.Create<Order> (Guid.NewGuid());
      _clientTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void GetObject_LoadsObjectIntoGivenTransaction ()
    {
      var result = _orderID1.GetObject (_clientTransaction);

      CheckDomainObject (result, _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
    }

    [Test]
    public void GetObject_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        var result = _orderID1.GetObject();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void GetObject_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderID1.GetObject(),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_LoadsDeletedObject ()
    {
      _clientTransaction.Execute (() => _orderID1.GetObject().Delete());

      var result = _orderID1.GetObject (_clientTransaction, includeDeleted: true);

      Assert.That (result, Is.Not.Null);
      CheckDomainObject (result, _clientTransaction, expectedState: StateType.Deleted);
    }

    [Test]
    public void GetObject_IncludeDeletedFalse_ThrowsOnDeletedObject ()
    {
      _clientTransaction.Execute (() => _orderID1.GetObject ().Delete ());
      Assert.That (() => _orderID1.GetObject (_clientTransaction, includeDeleted: false), Throws.TypeOf<ObjectDeletedException>());
    }

    [Test]
    public void GetObject_IncludeDeletedUnspecified_ThrowsOnDeletedObject ()
    {
      _clientTransaction.Execute (() => _orderID1.GetObject ().Delete ());
      Assert.That (() => _orderID1.GetObject (_clientTransaction), Throws.TypeOf<ObjectDeletedException> ());
    }

    [Test]
    public void TryGetObject_LoadsObjectIntoGivenTransaction ()
    {
      var result = _orderID1.TryGetObject (_clientTransaction);
      CheckDomainObject (result, _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
    }

    [Test]
    public void TryGetObject_AllowsNotFoundObjects ()
    {
      var result = _notFoundOrderID.TryGetObject (_clientTransaction);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryGetObject_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var result = _orderID1.TryGetObject ();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void TryGetObject_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderID1.TryGetObject (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjectReference_ReturnsReferenceFromGivenTransaction ()
    {
      var result = _orderID1.GetObjectReference (_clientTransaction);
      CheckDomainObject (result, _clientTransaction, expectedID: _orderID1, expectedState: StateType.NotLoadedYet);
    }

    [Test]
    public void GetObjectReference_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var result = _orderID1.GetObjectReference ();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void GetObjectReference_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderID1.GetObjectReference (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjects_LoadsObjectsIntoGivenTransaction ()
    {
      var results = new[] { _orderID1, _orderID2 }.GetObjects (_clientTransaction);

      Assert.That (results, Has.Length.EqualTo (2));
      CheckDomainObject (results[0], _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
      CheckDomainObject (results[1], _clientTransaction, expectedID: _orderID2, expectedState: StateType.Unchanged);
    }

    [Test]
    public void GetObjects_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var results = new[] { _orderID1 }.GetObjects ();

        CheckDomainObject (results[0], _clientTransaction);
      }
    }

    [Test]
    public void GetObjects_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => new[] { _orderID1 }.GetObjects (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjects_WithNotFound_Throws()
    {
      Assert.That (() => new[] { _notFoundOrderID }.GetObjects (_clientTransaction), Throws.TypeOf<ObjectsNotFoundException>());
    }

    [Test]
    public void TryGetObjects_LoadsObjectsIntoGivenTransaction ()
    {
      var results = new[] { _orderID1, _orderID2 }.TryGetObjects (_clientTransaction);

      Assert.That (results, Has.Length.EqualTo (2));
      CheckDomainObject (results[0], _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
      CheckDomainObject (results[1], _clientTransaction, expectedID: _orderID2, expectedState: StateType.Unchanged);
    }

    [Test]
    public void TryGetObjects_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var results = new[] { _orderID1 }.TryGetObjects ();
        CheckDomainObject (results[0], _clientTransaction);
      }
    }

    [Test]
    public void TryGetObjects_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => new[] { _orderID1 }.TryGetObjects (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void TryGetObjects_WithNotFound_Throws ()
    {
      var results = new[] { _notFoundOrderID }.TryGetObjects (_clientTransaction);
      Assert.That (results[0], Is.Null);
    }

    private void CheckDomainObject (
        DomainObject result,
        ClientTransaction expectedClientTransaction,
        IObjectID<DomainObject> expectedID = null,
        StateType? expectedState = null)
    {
      Assert.That (expectedClientTransaction.IsEnlisted (result), Is.True);
      if (expectedID != null)
        Assert.That (result.ID, Is.EqualTo (expectedID));
      if (expectedState != null)
        Assert.That (expectedClientTransaction.Execute (() => result.State), Is.EqualTo (expectedState));
    }
  }
}