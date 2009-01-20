// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class EventRaisingCollectionDataTest : ClientTransactionBaseTest
  {
    private readonly MockRepository _mockRepository = new MockRepository();

    private IDomainObjectCollectionEventRaiser _eventRaiserMock;
    private EventRaisingCollectionData _data;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;
    private OrderItem _orderItem1;

    public override void SetUp ()
    {
      base.SetUp();

      _eventRaiserMock = _mockRepository.StrictMock<IDomainObjectCollectionEventRaiser> ();
      _data = new EventRaisingCollectionData (_eventRaiserMock);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      _data.Insert (0, _order1);
      _data.Insert (1, _order2);
      _data.Insert (2, _order3);

      _eventRaiserMock.BackToRecord ();
    }

    [Test]
    public void Clear ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (0, _order1)).Do (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order2)).Do (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.BeginRemove (2, _order3)).Do (mi => Assert.That (_data.Count, Is.EqualTo (3)));

        _eventRaiserMock.Expect (mock => mock.EndRemove (2, _order3)).Do (mi => Assert.That (_data.Count, Is.EqualTo (0)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order2)).Do (mi => Assert.That (_data.Count, Is.EqualTo (0)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (0, _order1)).Do (mi => Assert.That (_data.Count, Is.EqualTo (0)));
      }

      _eventRaiserMock.Replay ();

      _data.Clear();

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Insert ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginAdd (2, _order4)).Do (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndAdd (2, _order4)).Do (mi => Assert.That (_data.Count, Is.EqualTo (4)));
      }

      _eventRaiserMock.Replay ();

      _data.Insert (2, _order4);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Insert_NoEventOnException ()
    {
      _eventRaiserMock.Replay ();

      try
      {
        _data.Insert (2, _order1);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException)
      {
      }

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Remove ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order2)).Do (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order2)).Do (mi => Assert.That (_data.Count, Is.EqualTo (2)));
      }

      _eventRaiserMock.Replay ();

      _data.Remove (_order2.ID);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      _eventRaiserMock.Replay ();

      _data.Remove (_order4.ID);

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order2)).Do (mi => Assert.That (_data.GetObject(1), Is.SameAs (_order2)));
        _eventRaiserMock.Expect (mock => mock.BeginAdd (1, _order4)).Do (mi => Assert.That (_data.GetObject (1), Is.SameAs (_order2)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order2)).Do (mi => Assert.That (_data.GetObject (1), Is.SameAs (_order4)));
        _eventRaiserMock.Expect (mock => mock.EndAdd (1, _order4)).Do (mi => Assert.That (_data.GetObject (1), Is.SameAs (_order4)));
      }

      _eventRaiserMock.Replay ();

      _data.Replace (_order2.ID, _order4);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Replace_NoEventsOnSelfReplace ()
    {
      _eventRaiserMock.Replay ();

      _data.Replace (_order2.ID, _order2);

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace_NoEventOnKeyNotFoundException ()
    {
      _eventRaiserMock.Replay ();

      try
      {
        _data.Replace (_order4.ID, _order2);
        Assert.Fail ("Expected exception");
      }
      catch (KeyNotFoundException)
      {
      }

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace_NoEventOnInvalidOperationException ()
    {
      _eventRaiserMock.Replay ();

      try
      {
        _data.Replace (_order2.ID, _order1);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException)
      {
      }

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }


    [Test]
    public void Serializable ()
    {
      var source = new EventRaisingCollectionData (new DomainObjectCollectionEventRaiserFake());
      source.Insert (0, _order1);
      source.Insert (1, _order2);
      source.Insert (2, _order3);

      var result = Serializer.SerializeAndDeserialize (source);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}