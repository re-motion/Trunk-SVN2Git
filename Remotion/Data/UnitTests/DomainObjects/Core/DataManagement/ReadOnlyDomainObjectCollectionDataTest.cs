// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ReadOnlyDomainObjectCollectionDataTest : ClientTransactionBaseTest
  {
    private ReadOnlyDomainObjectCollectionData _data;
    private IDomainObjectCollectionData _wrappedData;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;

    public override void SetUp ()
    {
      base.SetUp ();
      _wrappedData = new DomainObjectCollectionData ();
      _data = new ReadOnlyDomainObjectCollectionData (_wrappedData);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);

      _wrappedData.Insert (0, _order1);
      _wrappedData.Insert (1, _order2);
      _wrappedData.Insert (2, _order3);
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
      Assert.That (_data.IsReadOnly, Is.True);
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot clear a read-only collection.")]
    public void Clear_Throws ()
    {
      _data.Clear ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot insert an item into a read-only collection.")]
    public void Insert_Throws ()
    {
      _data.Insert (0, _order4);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void Remove_Throws ()
    {
      _data.Remove (_order1.ID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot replace an item in a read-only collection.")]
    public void Replace_Throws ()
    {
      _data.Replace (_order1.ID, _order1);
    }

    [Test]
    public void Serializable ()
    {
      var result = Serializer.SerializeAndDeserialize (_data);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}