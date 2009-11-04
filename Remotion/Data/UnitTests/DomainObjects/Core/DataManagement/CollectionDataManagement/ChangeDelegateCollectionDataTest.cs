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
// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class ChangeDelegateCollectionDataTest : ClientTransactionBaseTest
  {
    private ChangeDelegateCollectionData _data;
    private ICollectionChangeDelegate _changeDelegateMock;
    private IDomainObjectCollectionEventRaiser _eventRaiserMock;
    private DomainObjectCollection _parentCollection;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentCollection = new DomainObjectCollection ();

      var mockRepository = new MockRepository ();
      _changeDelegateMock = mockRepository.StrictMock<ICollectionChangeDelegate> ();
      _eventRaiserMock = mockRepository.StrictMock<IDomainObjectCollectionEventRaiser> ();

      _data = new ChangeDelegateCollectionData (_changeDelegateMock, _eventRaiserMock, _parentCollection);
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);

      _data.InsertData (0, _order1);
      _data.InsertData (1, _order2);
      _data.InsertData (2, _order3);
    }

    [Test]
    public void ChangeDelegate ()
    {
      Assert.That (_data.ChangeDelegate, Is.SameAs (_changeDelegateMock));
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
      Assert.That (_data.GetObject (_order1.ID), Is.SameAs (_order1));
    }

    [Test]
    public void IndexOf ()
    {
      Assert.That (_data.IndexOf (_order1.ID), Is.EqualTo (0));
      Assert.That (_data.IndexOf (_order2.ID), Is.EqualTo (1));
      Assert.That (_data.IndexOf (_order3.ID), Is.EqualTo (2));
      Assert.That (_data.IndexOf (_order4.ID), Is.EqualTo (-1));
    }

    [Test]
    public void GetEnumerator ()
    {
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void Clear ()
    {
      using (_changeDelegateMock.GetMockRepository ().Ordered ())
      {
        _changeDelegateMock.Expect (
            mock => mock.PerformRemove (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order3)));
        _changeDelegateMock.Expect (
            mock => mock.PerformRemove (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order2)));
        _changeDelegateMock.Expect (
            mock => mock.PerformRemove (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order1)));
      }

      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.Clear ();
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.VerifyAllExpectations ();

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Insert ()
    {
      _changeDelegateMock.Expect (
          mock =>
          mock.PerformInsert (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order4), Arg<int>.Is.Equal (0)));

      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.Insert (0, _order4);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.VerifyAllExpectations ();

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Remove ()
    {
      _changeDelegateMock.Expect (
        mock =>
        mock.PerformRemove (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order2)));

      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.Remove (_order2.ID);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.VerifyAllExpectations ();

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Remove_NonExistingObject ()
    {
      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.Remove (_order4.ID);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.AssertWasNotCalled (mock => mock.PerformRemove (Arg<DomainObjectCollection>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace ()
    {
      _changeDelegateMock.Expect (
        mock =>
        mock.PerformReplace (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order4), Arg<int>.Is.Equal (1)));

      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.Replace (_order2.ID, _order4);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.VerifyAllExpectations ();

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void SelfReplace ()
    {
      _changeDelegateMock.Expect (
        mock =>
        mock.PerformReplace (Arg<DomainObjectCollection>.Is.Same (_parentCollection), Arg<DomainObject>.Is.Same (_order2), Arg<int>.Is.Equal (1)));

      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.Replace (_order2.ID, _order2);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.VerifyAllExpectations ();

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void RaiseBeginAddEvent ()
    {
      _eventRaiserMock.Expect (mock => mock.BeginAdd (12, _order2));
      _eventRaiserMock.Replay ();

      _data.RaiseBeginAddEvent (12, _order2);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseEndAddEvent ()
    {
      _eventRaiserMock.Expect (mock => mock.EndAdd (12, _order2));
      _eventRaiserMock.Replay ();

      _data.RaiseEndAddEvent (12, _order2);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseBeginRemoveEvent ()
    {
      _eventRaiserMock.Expect (mock => mock.BeginRemove (12, _order2));
      _eventRaiserMock.Replay ();

      _data.RaiseBeginRemoveEvent (12, _order2);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseEndRemoveEvent ()
    {
      _eventRaiserMock.Expect (mock => mock.EndRemove (12, _order2));
      _eventRaiserMock.Replay ();

      _data.RaiseEndRemoveEvent (12, _order2);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void InsertData ()
    {
      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.InsertData (2, _order4);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order4, _order3 }));

      _changeDelegateMock.AssertWasNotCalled (
          mock => mock.PerformInsert (Arg<DomainObjectCollection>.Is.Anything, Arg<DomainObject>.Is.Anything, Arg<int>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void InsertData_Invalid ()
    {
      _data.InsertData (2, _order1);
    }

    [Test]
    public void RemoveData ()
    {
      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.RemoveData (_order2.ID);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order3 }));

      _changeDelegateMock.AssertWasNotCalled (
          mock => mock.PerformRemove (Arg<DomainObjectCollection>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void ReplaceData ()
    {
      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.ReplaceData (_order2.ID, _order4);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order4, _order3 }));

      _changeDelegateMock.AssertWasNotCalled (
          mock => mock.PerformReplace (Arg<DomainObjectCollection>.Is.Anything, Arg<DomainObject>.Is.Anything, Arg<int>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void ReplaceData_SelfReplace ()
    {
      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      _data.ReplaceData (_order2.ID, _order2);
      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));

      _changeDelegateMock.AssertWasNotCalled (
          mock => mock.PerformReplace (Arg<DomainObjectCollection>.Is.Anything, Arg<DomainObject>.Is.Anything, Arg<int>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void ReplaceData_Invalid ()
    {
      _changeDelegateMock.Replay ();
      _eventRaiserMock.Replay ();

      try
      {
        _data.ReplaceData (_order2.ID, _order1);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException)
      {
      }

      _changeDelegateMock.AssertWasNotCalled (
          mock => mock.PerformReplace (Arg<DomainObjectCollection>.Is.Anything, Arg<DomainObject>.Is.Anything, Arg<int>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }
  }
}
