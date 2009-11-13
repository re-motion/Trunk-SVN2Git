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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class TypeCheckingCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private IDomainObjectCollectionData _wrappedData;
    private TypeCheckingCollectionDataDecorator _typeCheckingDecorator;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;
    private OrderItem _orderItem1;

    public override void SetUp ()
    {
      base.SetUp ();

      _wrappedData = new DomainObjectCollectionData ();
      _typeCheckingDecorator = new TypeCheckingCollectionDataDecorator (_wrappedData, typeof (Order));

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      _wrappedData.Insert (0, _order1);
      _wrappedData.Insert (1, _order2);
      _wrappedData.Insert (2, _order3);
    }

    [Test]
    public void Enumeration ()
    {
      Assert.That (_typeCheckingDecorator.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void Count ()
    {
      Assert.That (_typeCheckingDecorator.Count, Is.EqualTo (3));
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_typeCheckingDecorator.IsReadOnly, Is.False);
    }

    [Test]
    public void ContainsObjectID ()
    {
      Assert.That (_typeCheckingDecorator.ContainsObjectID (_order1.ID), Is.True);
      Assert.That (_typeCheckingDecorator.ContainsObjectID (_order4.ID), Is.False);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      Assert.That (_typeCheckingDecorator.GetObject (0), Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_ByID ()
    {
      Assert.That (_typeCheckingDecorator.GetObject (_order2.ID), Is.SameAs (_order2));
    }

    [Test]
    public void IndexOf ()
    {
      Assert.That (_typeCheckingDecorator.IndexOf (_order2.ID), Is.EqualTo (1));
    }

    [Test]
    public void Clear ()
    {
      _typeCheckingDecorator.Clear ();
      Assert.That (_typeCheckingDecorator.ToArray(), Is.Empty);
      Assert.That (_wrappedData.ToArray (), Is.Empty);
    }

    [Test]
    public void Insert ()
    {
      _typeCheckingDecorator.Insert (0, _order4);
      Assert.That (_typeCheckingDecorator.ToArray (), Is.EqualTo (new[] { _order4, _order1, _order2, _order3 }));
      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _order4, _order1, _order2, _order3 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'" 
        + " cannot be added to this collection. Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' or derived from "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order'.\r\nParameter name: domainObject")]
    public void Insert_ThrowsOnInvalidType ()
    {
      _typeCheckingDecorator.Insert (0, _orderItem1);
    }

    [Test]
    public void Remove ()
    {
      _typeCheckingDecorator.Remove (_order1);
      Assert.That (_typeCheckingDecorator.ToArray (), Is.EqualTo (new[] { _order2, _order3 }));
      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _order2, _order3 }));
    }

    [Test]
    public void Remove_ID ()
    {
      _typeCheckingDecorator.Remove (_order1.ID);
      Assert.That (_typeCheckingDecorator.ToArray (), Is.EqualTo (new[] { _order2, _order3 }));
      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _order2, _order3 }));
    }

    [Test]
    public void Replace ()
    {
      _typeCheckingDecorator.Replace (0, _order4);
      Assert.That (_typeCheckingDecorator.ToArray (), Is.EqualTo (new[] { _order4, _order2, _order3 }));
      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _order4, _order2, _order3 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'"
        + " cannot be added to this collection. Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' or derived from "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order'.\r\nParameter name: newDomainObject")]
    public void Replace_ThrowsOnInvalidType ()
    {
      _typeCheckingDecorator.Replace (0, _orderItem1);
    }

    [Test]
    public void Serializable ()
    {
      var result = Serializer.SerializeAndDeserialize (_typeCheckingDecorator);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}
