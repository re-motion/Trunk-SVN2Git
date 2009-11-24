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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class SubCollectionEndPointChangeDetectionStrategyTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private CollectionEndPoint _customerEndPoint;
    private Order _order1; // Customer1
    private Order _orderWithoutOrderItem; // Customer1
    private Order _order2; // Customer3
    private SubCollectionEndPointChangeDetectionStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _customerEndPoint = new CollectionEndPoint (ClientTransactionMock, _customerEndPointID, new[] { _order1, _orderWithoutOrderItem });

      _strategy = new SubCollectionEndPointChangeDetectionStrategy ();
    }

    [Test]
    public void HasChanged_False ()
    {
      Assert.That (_strategy.HasChanged (_customerEndPoint), Is.False);
    }

    [Test]
    public void HasChanged_False_TouchedOnly ()
    {
      _customerEndPoint.Touch ();
      Assert.That (_strategy.HasChanged (_customerEndPoint), Is.False);
    }

    [Test]
    public void HasChanged_True_ReferenceChanged ()
    {
      _customerEndPoint.ReplaceOppositeCollection (_customerEndPoint.OppositeDomainObjects.Clone ());
      Assert.That (_strategy.HasChanged (_customerEndPoint), Is.True);
    }

    [Test]
    public void HasChanged_True_Content ()
    {
      _customerEndPoint.OppositeDomainObjects.Add (_order2);
      Assert.That (_strategy.HasChanged (_customerEndPoint), Is.True);
    }

    [Test]
    public void HasChanged_True_OrderOnly ()
    {
      _customerEndPoint.OppositeDomainObjects.RemoveAt (0);
      _customerEndPoint.OppositeDomainObjects.Insert (1, _order1);
      Assert.That (_strategy.HasChanged (_customerEndPoint), Is.True);
    }
  }
}