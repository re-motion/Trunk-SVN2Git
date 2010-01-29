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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using System.Linq;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class ReadOnlyDomainObjectCollectionAdapterTest : ClientTransactionBaseTest
  {
    private ReadOnlyDomainObjectCollectionAdapter<DomainObject> _readOnlyAdapter;
    private DomainObjectCollection _wrappedData;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp ();
      _wrappedData = new DomainObjectCollection (typeof (Order));
      _readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (_wrappedData);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);
    }

    [Test]
    public void Enumeration ()
    {
      StubInnerData (_order1, _order2, _order3);
      Assert.That (_readOnlyAdapter.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That (_readOnlyAdapter.RequiredItemType, Is.SameAs (typeof (Order)));
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      var associatedCollection = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      var readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (associatedCollection);
      
      Assert.That (readOnlyAdapter.AssociatedEndPoint, Is.SameAs (associatedCollection.AssociatedEndPoint));
    }

    [Test]
    public void IsDataAvailable ()
    {
      Assert.That (_readOnlyAdapter.IsDataAvailable, Is.True);

      var associatedCollection = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock, 
          associatedCollection.AssociatedEndPoint.ID, 
          UnloadService.TransactionMode.ThisTransactionOnly);

      var readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (associatedCollection);
      Assert.That (readOnlyAdapter.IsDataAvailable, Is.False);
    }

    [Test]
    public void Count ()
    {
      StubInnerData (_order1, _order2, _order3);
      Assert.That (_readOnlyAdapter.Count, Is.EqualTo (3));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      var associatedCollection = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock,
          associatedCollection.AssociatedEndPoint.ID,
          UnloadService.TransactionMode.ThisTransactionOnly);

      var readOnlyAdapter = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (associatedCollection);
      Assert.That (associatedCollection.IsDataAvailable, Is.False);

      readOnlyAdapter.EnsureDataAvailable ();

      Assert.That (associatedCollection.IsDataAvailable, Is.True);
    }

    [Test]
    public void Contains_ID ()
    {
      StubInnerData (_order1, _order2, _order3);

      Assert.That (_readOnlyAdapter.Contains (_order1.ID), Is.True);
      Assert.That (_readOnlyAdapter.Contains (_order4.ID), Is.False);
    }

    [Test]
    public void ContainsObject ()
    {
      StubInnerData (_order1, _order2, _order3);

      Assert.That (_readOnlyAdapter.ContainsObject (_order1), Is.True);
      Assert.That (_readOnlyAdapter.ContainsObject (_order4), Is.False);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      StubInnerData (_order1, _order2, _order3);

      Assert.That (_readOnlyAdapter.GetObject (0), Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      StubInnerData (_order1, _order2, _order3);

      Assert.That (_readOnlyAdapter.GetObject (_order2.ID), Is.SameAs (_order2));
    }

    [Test]
    public void IndexOf_ID ()
    {
      StubInnerData (_order1, _order2, _order3);

      Assert.That (_readOnlyAdapter.IndexOf (_order2.ID), Is.EqualTo (1));
    }

    [Test]
    public void IndexOf_Object ()
    {
      StubInnerData (_order1, _order2, _order3);

      Assert.That (_readOnlyAdapter.IndexOf (_order2), Is.EqualTo (1));
    }

    [Test]
    public void Serializable ()
    {
      StubInnerData (_order1, _order2, _order3);
      var result = Serializer.SerializeAndDeserialize (_readOnlyAdapter);
      Assert.That (result.Count, Is.EqualTo (3));
    }

    private void StubInnerData (params DomainObject[] contents)
    {
      _wrappedData.AddRange (contents);
    }
  }
}
