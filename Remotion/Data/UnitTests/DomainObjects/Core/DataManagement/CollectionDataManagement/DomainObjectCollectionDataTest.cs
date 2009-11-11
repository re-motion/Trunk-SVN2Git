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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class DomainObjectCollectionDataTest : ClientTransactionBaseTest
  {
    private DomainObjectCollectionData _data;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp ();
      _data = new DomainObjectCollectionData ();
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);
    }

    [Test]
    public void Initialization ()
    {
      var data = new DomainObjectCollectionData ();
      Assert.That (data.ToArray (), Is.Empty);
      Assert.That (data.Count, Is.EqualTo (0));
    }

    [Test]
    public void Initialization_WithValues ()
    {
      var data = new DomainObjectCollectionData (new[] {_order1, _order2, _order3});
      Assert.That (data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (data.Count, Is.EqualTo (3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_data.IsReadOnly, Is.False);
    }

    [Test]
    public void Insert ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Insert (2, _order4);

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order4, _order3 }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The collection already contains an object with ID "
                                                                              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void Insert_SameItemTwice ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Insert (2, _order1);
    }

    [Test]
    public void Insert_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Insert (2, _order4));
    }


    [Test]
    public void ContainsObjectID_False ()
    {
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void ContainsObjectID_True ()
    {
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.Order1), Is.False);
      Add (_order1);
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void GetObject_ByID ()
    {
      Add (_order1);
      Add (_order2);

      Assert.That (_data.GetObject (_order1.ID), Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_InvalidID ()
    {
      Assert.That (_data.GetObject (_order1.ID), Is.Null);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      Add (_order1);
      Add (_order2);

      Assert.That (_data.GetObject (0), Is.SameAs (_order1));
      Assert.That (_data.GetObject (1), Is.SameAs (_order2));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetObject_InvalidIndex ()
    {
      Add (_order1);
      Add (_order2);

      _data.GetObject (3);
    }

    [Test]
    public void IndexOf ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert.That (_data.IndexOf (_order1.ID), Is.EqualTo (0));
      Assert.That (_data.IndexOf (_order2.ID), Is.EqualTo (1));
      Assert.That (_data.IndexOf (_order3.ID), Is.EqualTo (2));
      Assert.That (_data.IndexOf (_order4.ID), Is.EqualTo (-1));
    }

    [Test]
    public void Clear ()
    {
      Add (_order1);
      Add (_order2);

      _data.Clear ();

      Assert.That (_data.Count, Is.EqualTo (0));
      Assert.That (_data.ToArray(), Is.Empty);
    }

    [Test]
    public void Clear_AlsoClearsObjectsByID ()
    {
      Add (_order1);
      Add (_order2);

      _data.Clear ();

      Assert.That (_data.ContainsObjectID (_order1.ID), Is.False);
    }

    [Test]
    public void Clear_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Clear());
    }

    [Test]
    public void Remove ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Remove (_order2);
      Assert.That (_data.ToArray(), Is.EqualTo (new[] { _order1, _order3 }));
    }

    [Test]
    public void Remove_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Remove (_order3));
    }

    [Test]
    public void Remove_NonExistingElement ()
    {
      _data.Remove (_order2);
      Assert.That (_data.ToArray (), Is.Empty);
    }

    [Test]
    public void Remove_NonExistingElement_NoVersionChange ()
    {
      long oldVersion = _data.Version;
      _data.Remove (_order2);
      Assert.That (_data.Version, Is.EqualTo (oldVersion));
    }

    [Test]
    public void Replace ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Replace (_order2.ID, _order4);

      Assert.That (_data.ToArray(), Is.EqualTo (new[] { _order1, _order4, _order3 }));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "The collection does not contain a DomainObject with ID "
                                                                         + "'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'.")]
    public void Replace_NonExisting ()
    {
      _data.Replace (_order2.ID, _order4);
    }

    [Test]
    public void Replace_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Replace (_order2.ID, _order4));
    }

    [Test]
    public void Replace_NoVersionChange_OnSelfReplace ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      var oldVersion = _data.Version;
      _data.Replace (_order2.ID, _order2);
      Assert.That (_data.Version, Is.EqualTo (oldVersion));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The collection already contains an object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void Replace_WithDuplicate ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Replace (_order2.ID, _order1);
    }

    [Test]
    public void Replace_WithSelf ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Replace (_order2.ID, _order2);

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Collection was modified during enumeration.")]
    public void Enumeration_ChokesOnVersionChanges ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      var enumerator = _data.GetEnumerator ();
      enumerator.MoveNext();

      Add (_order4);

      enumerator.MoveNext ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Collection was modified during enumeration.")]
    public void Enumeration_ChokesOnVersionChanges_Remove ()
    {
      Add (_order1);
      Add (_order2);

      foreach (var x in _data)
        _data.Remove (x);
    }

    [Test]
    public void Serializable ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      var result = Serializer.SerializeAndDeserialize (_data);
      Assert.That (result.Count, Is.EqualTo (3));
    }

    private void Add (Order order)
    {
      _data.Insert (_data.Count, order);
    }

    private void Assert_VersionChanged (Action action)
    {
      long version = _data.Version;
      action ();
      Assert.That (_data.Version, Is.GreaterThan (version));
    }
  }
}
