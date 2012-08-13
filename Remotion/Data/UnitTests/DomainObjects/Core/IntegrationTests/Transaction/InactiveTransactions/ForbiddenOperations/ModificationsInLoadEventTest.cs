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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.ForbiddenOperations
{
  [TestFixture]
  [Ignore ("TODO 4994")]
  public class LoadEventsTest : InactiveTransactionsTestBase
  {
    private Order _order;
    private Location _location;
    private ILoadEventReceiver _loadEventReceiverMock;
    private Client _client1;
    private Client _client2;
    private Client _client3;
    private Client _client4;
    private OrderTicket _orderTicket1;

    public override void SetUp ()
    {
      base.SetUp();

      _order = (Order) LifetimeService.GetObjectReference (ActiveSubTransaction, DomainObjectIDs.Order1);
      _location = (Location) LifetimeService.GetObjectReference (ActiveSubTransaction, DomainObjectIDs.Location1);
      _client1 = (Client) LifetimeService.GetObject (ActiveSubTransaction, DomainObjectIDs.Client1, false);
      _client2 = (Client) LifetimeService.GetObject (ActiveSubTransaction, DomainObjectIDs.Client2, false);
      _client3 = (Client) LifetimeService.GetObject (ActiveSubTransaction, DomainObjectIDs.Client3, false);
      _client4 = (Client) LifetimeService.NewObject (ActiveSubTransaction, typeof (Client), ParamList.Empty);
      _orderTicket1 = (OrderTicket) LifetimeService.GetObject (ActiveSubTransaction, DomainObjectIDs.OrderTicket1, false);

      _loadEventReceiverMock = MockRepository.GenerateStrictMock<ILoadEventReceiver>();
      _order.SetLoadEventReceiver (_loadEventReceiverMock);
      _location.SetLoadEventReceiver (_loadEventReceiverMock);
    }

    [Test]
    public void OnLoaded_CannotModifyOtherObject_PropertyValues ()
    {
      // No load events for _order
      _order.SetLoadEventReceiver (null);

      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Load _location, but try to modify _order
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveRootTransaction, _location, _order, o => o.OrderNumber, newValue: 2));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveMiddleTransaction, _location, _order, o => o.OrderNumber, newValue: 3));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckForbiddenSetProperty (ActiveSubTransaction, _location, _order, o => o.OrderNumber, newValue: 4));

        ActiveSubTransaction.EnsureDataAvailable (_location.ID);

        _loadEventReceiverMock.VerifyAllExpectations();

        CheckProperty (InactiveRootTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 1);
        CheckProperty (InactiveMiddleTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 1);
        CheckProperty (ActiveSubTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 1);
      }
    }

    [Test]
    public void OnLoaded_CannotModifyOtherObject_UnidirectionalRelations ()
    {
      // No load events for _location
      _location.SetLoadEventReceiver (null);

      using (_loadEventReceiverMock.GetMockRepository().Ordered())
      {
        // Load _order, but try to modify _location
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveRootTransaction, _order, _location, l => l.Client, newValue: _client2));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveMiddleTransaction, _order, _location, l => l.Client, newValue: _client3));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckForbiddenSetProperty (ActiveSubTransaction, _order, _location, l => l.Client, newValue: _client4));

        ActiveSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations();

        CheckProperty (InactiveRootTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client1);
        CheckProperty (InactiveMiddleTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client1);
        CheckProperty (ActiveSubTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client1);
      }
    }

    [Test]
    public void OnLoaded_CannotModifyOtherObject_BidirectionalRelations ()
    {
      // No load events for _location
      _order.SetLoadEventReceiver (null);

      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Load _location, but try to modify _order
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveRootTransaction, _location, _order, o => o.OrderTicket, newValue: null));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveMiddleTransaction, _location, _order, o => o.OrderTicket, newValue: null));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckForbiddenSetProperty (ActiveSubTransaction, _location, _order, o => o.OrderTicket, newValue: null));

        ActiveSubTransaction.EnsureDataAvailable (_location.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckProperty (InactiveRootTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (InactiveMiddleTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (ActiveSubTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
      }
    }

    [Test]
    public void OnLoaded_CannotModifyThisObject_BidirectionalRelations ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Load _order, but trying to modify _order.OrderTicket would also modify _orderTicket1
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveRootTransaction, _order, _order, o => o.OrderTicket, newValue: null, offendingObject: _orderTicket1));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckForbiddenSetProperty (InactiveMiddleTransaction, _order, _order, o => o.OrderTicket, newValue: null, offendingObject: _orderTicket1));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckForbiddenSetProperty (ActiveSubTransaction, _order, _order, o => o.OrderTicket, newValue: null, offendingObject: _orderTicket1));

        ActiveSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckProperty (InactiveRootTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (InactiveMiddleTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (ActiveSubTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
      }
    }

    private void CheckForbiddenSetProperty<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        DomainObject loadedObject,
        TDomainObject modifiedObject,
        Expression<Func<TDomainObject, TValue>> propertyExpression,
        TValue newValue,
        DomainObject offendingObject = null)
      where TDomainObject : DomainObject
    {
      offendingObject = offendingObject ?? modifiedObject;

      Assert.That (ClientTransaction.Current, Is.SameAs (clientTransaction));
      var expected = string.Format (
          "While the object '{0}' is being loaded, only this object can be modified. The object '{1}' cannot be modified.",
          loadedObject.ID,
          offendingObject.ID);
      Assert.That (
          () => SetProperty (clientTransaction, modifiedObject, propertyExpression, newValue),
          Throws.InvalidOperationException.With.Message.EqualTo (expected));
    }
  }
}