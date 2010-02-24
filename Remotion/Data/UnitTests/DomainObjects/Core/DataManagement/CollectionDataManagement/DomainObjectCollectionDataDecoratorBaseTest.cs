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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class DomainObjectCollectionDataDecoratorBaseTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    private IDomainObjectCollectionData _wrappedDataMock;
    private TestDomainObjectCollectionDecorator _decorator;


    private Order _order1;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      _wrappedDataMock = _mockRepository.StrictMock<IDomainObjectCollectionData> ();

      _decorator = new TestDomainObjectCollectionDecorator (_wrappedDataMock);

      _order1 = DomainObjectMother.CreateFakeObject<Order> ();
      _order2 = DomainObjectMother.CreateFakeObject<Order> ();
    }

    [Test]
    public void GetEnumerator ()
    {
      using (IEnumerator<DomainObject> fakeEnumerator = new List<DomainObject> ().GetEnumerator ())
      {
        _wrappedDataMock.Expect (mock => mock.GetEnumerator()).Return (fakeEnumerator);
        _wrappedDataMock.Replay();

        var result = _decorator.GetEnumerator();

        _wrappedDataMock.VerifyAllExpectations ();
        Assert.That (result, Is.SameAs (fakeEnumerator));
      }
    }

    [Test]
    public void Count ()
    {
      _wrappedDataMock.Expect (mock => mock.Count).Return (7);
      _wrappedDataMock.Replay ();

      var result = _decorator.Count;

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (7));
    }

    [Test]
    public void IsReadOnly ()
    {
      _wrappedDataMock.Expect (mock => mock.IsReadOnly).Return (true);
      _wrappedDataMock.Replay ();

      var result = _decorator.IsReadOnly;

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      var fakeEndPoint = _mockRepository.Stub<ICollectionEndPoint> ();

      _wrappedDataMock.Expect (mock => mock.AssociatedEndPoint).Return (fakeEndPoint);
      _wrappedDataMock.Replay ();

      var result = _decorator.AssociatedEndPoint;

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeEndPoint));
    }

    [Test]
    public void IsDataAvailable ()
    {
      _wrappedDataMock.Expect (mock => mock.IsDataAvailable).Return (true);
      _wrappedDataMock.Replay ();

      var result = _decorator.IsDataAvailable;

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      _wrappedDataMock.Expect (mock => mock.EnsureDataAvailable());
      _wrappedDataMock.Replay ();

      _decorator.EnsureDataAvailable();

      _wrappedDataMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetDataStore ()
    {
      var fakeDataStore = _mockRepository.Stub<IDomainObjectCollectionData> ();

      _wrappedDataMock.Expect (mock => mock.GetDataStore()).Return (fakeDataStore);
      _wrappedDataMock.Replay ();

      var result = _decorator.GetDataStore();

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeDataStore));
    }

    [Test]
    public void ContainsObjectID ()
    {
      _wrappedDataMock.Expect (mock => mock.ContainsObjectID (_order1.ID)).Return (true);
      _wrappedDataMock.Replay ();

      var result = _decorator.ContainsObjectID (_order1.ID);

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      _wrappedDataMock.Expect (mock => mock.GetObject (1)).Return (_order1);
      _wrappedDataMock.Replay ();

      var result = _decorator.GetObject (1);

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      _wrappedDataMock.Expect (mock => mock.GetObject (_order2.ID)).Return (_order2);
      _wrappedDataMock.Replay ();

      var result = _decorator.GetObject (_order2.ID);

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_order2));
    }

    [Test]
    public void IndexOf ()
    {
      _wrappedDataMock.Expect (mock => mock.IndexOf (_order2.ID)).Return (47);
      _wrappedDataMock.Replay ();

      var result = _decorator.IndexOf (_order2.ID);

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (47));
    }

    [Test]
    public void Clear ()
    {
      _wrappedDataMock.Expect (mock => mock.Clear());
      _wrappedDataMock.Replay ();

      _decorator.Clear();

      _wrappedDataMock.VerifyAllExpectations ();
    }

    [Test]
    public void Insert ()
    {
      _wrappedDataMock.Expect (mock => mock.Insert (13, _order1));
      _wrappedDataMock.Replay ();

      _decorator.Insert (13, _order1);

      _wrappedDataMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove ()
    {
      _wrappedDataMock.Expect (mock => mock.Remove (_order2)).Return (false);
      _wrappedDataMock.Replay ();

      var result = _decorator.Remove (_order2);

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo(false));
    }

    [Test]
    public void Remove_ID ()
    {
      _wrappedDataMock.Expect (mock => mock.Remove (_order2.ID)).Return (false);
      _wrappedDataMock.Replay ();

      var result = _decorator.Remove (_order2.ID);

      _wrappedDataMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (false));
    }

    [Test]
    public void Replace ()
    {
      _wrappedDataMock.Expect (mock => mock.Replace (10, _order2));
      _wrappedDataMock.Replay ();

      _decorator.Replace (10, _order2);

      _wrappedDataMock.VerifyAllExpectations ();
    }

    [Test]
    public void Serializable ()
    {
      var source = new TestDomainObjectCollectionDecorator (new DomainObjectCollectionData (new[] { _order1, _order2 }));

      var result = Serializer.SerializeAndDeserialize (source);
      Assert.That (result.Count, Is.EqualTo (2));
    }
  }
}
