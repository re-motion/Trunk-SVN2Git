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
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class ArgumentCheckingCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private IDomainObjectCollectionData _wrappedDataMock;
    private ArgumentCheckingCollectionDataDecorator _argumentCheckingDecorator;

    private Order _order1;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp ();

      _wrappedDataMock = MockRepository.GenerateMock<IDomainObjectCollectionData> ();
      _argumentCheckingDecorator = new ArgumentCheckingCollectionDataDecorator (_wrappedDataMock);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
    }

    [Test]
    public void TrivialMembers_Delegated ()
    {
      CheckDelegation (data => data.GetEnumerator ());
      CheckDelegation (data => Dev.Null = data.Count);
      CheckDelegation (data => Dev.Null = data.IsReadOnly);
      CheckDelegation (data => data.ContainsObjectID (DomainObjectIDs.Order1));
      CheckDelegation (data => data.GetObject (0));
      CheckDelegation (data => data.GetObject (DomainObjectIDs.Order1));
      CheckDelegation (data => data.Clear());
      CheckDelegation (data => data.Remove (DomainObjectIDs.Order1));
    }

    [Test]
    public void Insert ()
    {
      StubInnerData (_order1);

      _argumentCheckingDecorator.Insert (0, _order2);

      _wrappedDataMock.AssertWasCalled (mock => mock.Insert (0, _order2));
    }

    [Test]
    public void Insert_Duplicate ()
    {
      StubInnerData (_order1);

      CheckThrows<InvalidOperationException> (
          () => _argumentCheckingDecorator.Insert (0, _order1), 
          "The collection already contains an object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Insert_IndexTooHigh ()
    {
      CheckThrows<ArgumentOutOfRangeException> (
          () => _argumentCheckingDecorator.Insert (1, _order1),
          "Index is out of range. Must be non-negative and less than or equal to the size of the collection.\r\nParameter name: index\r\nActual value was 1.");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Insert_IndexTooLow ()
    {
      CheckThrows<ArgumentOutOfRangeException> (
          () => _argumentCheckingDecorator.Insert (-1, _order1),
          "Index is out of range. Must be non-negative and less than or equal to the size of the collection.\r\nParameter name: index\r\nActual value was -1.");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Remove ()
    {
      StubInnerData (_order1);

      _argumentCheckingDecorator.Remove (_order1);

      _wrappedDataMock.AssertWasCalled (mock => mock.Remove (_order1));
    }

    [Test]
    public void Remove_HoldsObjectFromOtherTransaction ()
    {
      _wrappedDataMock.Stub (stub => stub.GetObject (DomainObjectIDs.Order1)).Return (_order2);

      CheckThrows<ArgumentException> (
          () => _argumentCheckingDecorator.Remove (_order1),
          "The object to be removed has the same ID as an object in this collection, but is a different object reference.\r\nParameter name: domainObject");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Remove (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace ()
    {
      StubInnerData (_order1);

      _argumentCheckingDecorator.Replace (0, _order2);

      _wrappedDataMock.AssertWasCalled (mock => mock.Replace (0, _order2));
    }

    [Test]
    public void Replace_WithSameObject ()
    {
      StubInnerData (_order1);

      _argumentCheckingDecorator.Replace (0, _order1);

      _wrappedDataMock.AssertWasCalled (mock => mock.Replace (0, _order1));
    }

    [Test]
    public void Replace_WithDuplicate ()
    {
      StubInnerData (_order1, _order2);

      CheckThrows<InvalidOperationException> (
          () => _argumentCheckingDecorator.Replace (1, _order1),
          "The collection already contains an object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Replace (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace_IndexTooLow ()
    {
      StubInnerData (_order1);

      CheckThrows<ArgumentOutOfRangeException> (
          () => _argumentCheckingDecorator.Replace (-1, _order1),
          "Index is out of range. Must be non-negative and less than the size of the collection.\r\nParameter name: index\r\nActual value was -1.");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Replace (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace_IndexTooHigh ()
    {
      StubInnerData (_order1);

      CheckThrows<ArgumentOutOfRangeException> (
          () => _argumentCheckingDecorator.Replace (1, _order1),
          "Index is out of range. Must be non-negative and less than the size of the collection.\r\nParameter name: index\r\nActual value was 1.");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Replace (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Serializable ()
    {
      var decorator = new ArgumentCheckingCollectionDataDecorator (new DomainObjectCollectionData(new[] { _order1, _order2 }));
      var deserializedDecorator = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserializedDecorator.Count(), Is.EqualTo (2));
    }

    private void CheckDelegation (Action<IDomainObjectCollectionData> action)
    {
      action (_argumentCheckingDecorator);
      _wrappedDataMock.AssertWasCalled (action);
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

    private void StubInnerData (params DomainObject[] contents)
    {
      _wrappedDataMock.Stub (stub => stub.Count).Return (contents.Length);

      for (int i = 0; i < contents.Length; i++)
      {
        int currentIndex = i; // required because Stub creates a closure
        _wrappedDataMock.Stub (stub => stub.ContainsObjectID (contents[currentIndex].ID)).Return (true);
        _wrappedDataMock.Stub (stub => stub.GetObject (contents[currentIndex].ID)).Return (contents[currentIndex]);
        _wrappedDataMock.Stub (stub => stub.GetObject (currentIndex)).Return (contents[currentIndex]);
        _wrappedDataMock.Stub (stub => stub.IndexOf (contents[currentIndex].ID)).Return (currentIndex);
      }
    }
  }
}