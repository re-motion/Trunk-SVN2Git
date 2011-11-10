// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionData
{
  [TestFixture]
  public class ObservableCollectionDataDecoratorBaseTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private TestableObservableCollectionDataDecorator.IEventSink _eventSinkMock;

    private TestableObservableCollectionDataDecorator _observableDecoratorWithRealContent;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);

      var realContent = new DomainObjectCollectionData (new[] { _order1, _order2, _order3 });

      _eventSinkMock = _mockRepository.StrictMock<TestableObservableCollectionDataDecorator.IEventSink>();
      _observableDecoratorWithRealContent = new TestableObservableCollectionDataDecorator (realContent, _eventSinkMock);

      _eventSinkMock.Replay();
    }

    [Test]
    public void Clear ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order1, 0))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));

        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (0)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (0)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order1, 0))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (0)));
      }

      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Clear();

      _eventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Insert ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order4, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order4, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (4)));
      }

      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Insert (2, _order4);

      _eventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Remove ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (2)));
      }

      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order2);

      _eventSinkMock.VerifyAllExpectations();

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order4);

      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg <ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ID ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (2)));
      }

      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order2.ID);

      _eventSinkMock.VerifyAllExpectations();

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_NoEventIfNoRemove ()
    {
      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order4.ID);

      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));

      Assert.That (result, Is.False);
    }

    [Test]
    public void Replace ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order2)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order4, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order2)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order2, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order4)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order4, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order4)));
      }

      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Replace (1, _order4);

      _eventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Replace_NoEventsOnSelfReplace ()
    {
      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Replace (1, _order2);

      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg < ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
    }

    [Test]
    public void Serializable ()
    {
      var source = new TestableObservableCollectionDataDecorator (new DomainObjectCollectionData(), null);
      source.Insert (0, _order1);
      source.Insert (1, _order2);
      source.Insert (2, _order3);

      var result = Serializer.SerializeAndDeserialize (source);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}