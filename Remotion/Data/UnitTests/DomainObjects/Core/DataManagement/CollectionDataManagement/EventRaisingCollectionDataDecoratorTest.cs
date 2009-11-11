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
  public class EventRaisingCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private readonly MockRepository _mockRepository = new MockRepository();

    private IDomainObjectCollectionEventRaiser _eventRaiserMock;
    private EventRaisingCollectionDataDecorator _data;
    private IDomainObjectCollectionData _wrappedData;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp();

      _eventRaiserMock = _mockRepository.StrictMock<IDomainObjectCollectionEventRaiser> ();
      
      _wrappedData = new DomainObjectCollectionData ();
      _data = new EventRaisingCollectionDataDecorator (_eventRaiserMock, _wrappedData);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);

      _wrappedData.Insert (0, _order1);
      _wrappedData.Insert (1, _order2);
      _wrappedData.Insert (2, _order3);

      _eventRaiserMock.BackToRecord ();
      _eventRaiserMock.Replay();
    }

    [Test]
    public void Enumeration ()
    {
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void Count ()
    {
      Assert.That (_data.Count, Is.EqualTo (3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_data.IsReadOnly, Is.False);
    }

    [Test]
    public void ContainsObjectID ()
    {
      Assert.That (_data.ContainsObjectID (_order1.ID), Is.True);
      Assert.That (_data.ContainsObjectID (_order4.ID), Is.False);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      Assert.That (_data.GetObject (0), Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      Assert.That (_data.GetObject (_order2.ID), Is.SameAs (_order2));
    }

    [Test]
    public void IndexOf ()
    {
      Assert.That (_data.IndexOf (_order2.ID), Is.EqualTo (1));
    }

    [Test]
    public void Clear ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (0, _order1)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order2)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.BeginRemove (2, _order3)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (3)));

        _eventRaiserMock.Expect (mock => mock.EndRemove (2, _order3)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (0)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order2)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (0)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (0, _order1)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (0)));
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
        _eventRaiserMock.Expect (mock => mock.BeginAdd (2, _order4)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndAdd (2, _order4)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (4)));
      }

      _eventRaiserMock.Replay ();

      _data.Insert (2, _order4);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order2)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order2)).WhenCalled (mi => Assert.That (_data.Count, Is.EqualTo (2)));
      }

      _eventRaiserMock.Replay ();

      _data.Remove (_order2);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      _eventRaiserMock.Replay ();

      _data.Remove (_order4);

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order2)).WhenCalled (mi => Assert.That (_data.GetObject(1), Is.SameAs (_order2)));
        _eventRaiserMock.Expect (mock => mock.BeginAdd (1, _order4)).WhenCalled (mi => Assert.That (_data.GetObject (1), Is.SameAs (_order2)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order2)).WhenCalled (mi => Assert.That (_data.GetObject (1), Is.SameAs (_order4)));
        _eventRaiserMock.Expect (mock => mock.EndAdd (1, _order4)).WhenCalled (mi => Assert.That (_data.GetObject (1), Is.SameAs (_order4)));
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
    public void Serializable ()
    {
      var source = new EventRaisingCollectionDataDecorator (new DomainObjectCollectionEventRaiserFake(), new DomainObjectCollectionData());
      source.Insert (0, _order1);
      source.Insert (1, _order2);
      source.Insert (2, _order3);

      var result = Serializer.SerializeAndDeserialize (source);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}
