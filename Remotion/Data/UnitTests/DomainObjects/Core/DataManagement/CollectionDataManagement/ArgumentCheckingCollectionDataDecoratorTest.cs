// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class ArgumentCheckingCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private IDomainObjectCollectionData _innerDataMock;
    private ArgumentCheckingCollectionDataDecorator _argumentCheckingDataDecorator;

    private Order _order1;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp ();

      _innerDataMock = MockRepository.GenerateMock<IDomainObjectCollectionData> ();
      _argumentCheckingDataDecorator = new ArgumentCheckingCollectionDataDecorator (_innerDataMock);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
    }

    [Test]
    public void AllMembers_Delegated ()
    {
      CheckDelegation (data => data.GetEnumerator ());
      CheckDelegation (data => Dev.Null = data.Count);
      CheckDelegation (data => Dev.Null = data.IsReadOnly);
      CheckDelegation (data => data.ContainsObjectID (DomainObjectIDs.Order1));
      CheckDelegation (data => data.GetObject (0));
      CheckDelegation (data => data.GetObject (DomainObjectIDs.Order1));
      CheckDelegation (data => data.Clear());
      CheckDelegation (data => data.Insert (0, _order1));
      CheckDelegation (data => data.Remove (_order1));
      CheckDelegation (data => data.Remove (DomainObjectIDs.Order1));
      CheckDelegation (data => data.Replace (0, _order1));
    }

    [Test]
    public void Insert_Duplicate ()
    {
      _innerDataMock.Stub (stub => stub.ContainsObjectID (DomainObjectIDs.Order1)).Return (true);

      CheckThrows<InvalidOperationException> (
          () => _argumentCheckingDataDecorator.Insert (0, _order1), 
          "The collection already contains an object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.");

      _innerDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Insert_IndexTooHigh ()
    {
      _innerDataMock.Stub (stub => stub.ContainsObjectID (DomainObjectIDs.Order1)).Return (false);
      _innerDataMock.Stub (stub => stub.Count).Return (0);

      CheckThrows<ArgumentOutOfRangeException> (
          () => _argumentCheckingDataDecorator.Insert (1, _order1),
          "Index is out of range. Must be non-negative and less than or equal to the size of the collection.\r\nParameter name: index\r\nActual value was 1.");

      _innerDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Insert_IndexTooLow ()
    {
      _innerDataMock.Stub (stub => stub.ContainsObjectID (DomainObjectIDs.Order1)).Return (false);
      _innerDataMock.Stub (stub => stub.Count).Return (0);

      CheckThrows<ArgumentOutOfRangeException> (
          () => _argumentCheckingDataDecorator.Insert (-1, _order1),
          "Index is out of range. Must be non-negative and less than or equal to the size of the collection.\r\nParameter name: index\r\nActual value was -1.");

      _innerDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Remove_HoldsObjectFromOtherTransaction ()
    {
      _innerDataMock.Stub (stub => stub.GetObject (DomainObjectIDs.Order1)).Return (_order2);

      CheckThrows<ArgumentException> (
          () => _argumentCheckingDataDecorator.Remove (_order1),
          "The object to be removed has the same ID as an object in this collection, but is a different object reference.\r\nParameter name: domainObject");

      _innerDataMock.AssertWasNotCalled (mock => mock.Remove (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    [Ignore ("TODO 1779")]
    public void Replace ()
    {
      Assert.Fail ("TODO");
    }

    private void CheckDelegation (Action<IDomainObjectCollectionData> action)
    {
      action (_argumentCheckingDataDecorator);
      _innerDataMock.AssertWasCalled (action);
    }

    private void CheckThrows<T> (Action action, string expectedMessage) where T : Exception
    {
      try
      {
        action ();
      }
      catch (T ex)
      {
        Assert.That (ex.Message, Is.EqualTo (expectedMessage), "Exception message doesn't match.");
        return;
      }
      catch (Exception ex)
      {
        Assert.Fail ("Expected " + typeof (T) + ", got " + ex);
      }
      Assert.Fail ("Expected " + typeof (T));
    }
  }
}