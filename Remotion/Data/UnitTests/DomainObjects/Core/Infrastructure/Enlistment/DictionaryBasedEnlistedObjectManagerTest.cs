// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Enlistment
{
  [TestFixture]
  public class DictionaryBasedEnlistedObjectManagerTest : StandardMappingTest
  {
    private DictionaryBasedEnlistedObjectManager<ObjectID, DomainObject> _manager;
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp ();

      _manager = new DictionaryBasedEnlistedObjectManager<ObjectID, DomainObject> (obj => obj.ID);
      _order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
    }

    [Test]
    public void EnlistDomainObject ()
    {
      Assert.That (_manager.IsEnlisted (_order), Is.False);

      var result = _manager.EnlistObject (_order);

      Assert.That (result, Is.True);
      Assert.That (_manager.IsEnlisted (_order), Is.True);
    }

    [Test]
    public void EnlistDomainObject_Twice_WithSameObject ()
    {
      _manager.EnlistObject (_order);
      var result = _manager.EnlistObject (_order);

      Assert.That (result, Is.False);
      Assert.That (_manager.IsEnlisted (_order), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void EnlistDomainObject_IDAlreadyEnlisted ()
    {
      var orderA = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var orderB = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      _manager.EnlistObject (orderA);
      _manager.EnlistObject (orderB);
    }

    [Test]
    public void EnlistedDomainObjectCount ()
    {
      Assert.That (_manager.EnlistedObjectCount, Is.EqualTo (0));

      _manager.EnlistObject (_order);

      Assert.That (_manager.EnlistedObjectCount, Is.EqualTo (1));
    }

    [Test]
    public void GetEnlistedDomainObjects ()
    {
      _manager.EnlistObject (_order);

      Assert.That (_manager.GetEnlistedObjects ().ToArray(), Is.EqualTo (new[] { _order }));
    }

    [Test]
    public void GetEnlistedDomainObject ()
    {
      _manager.EnlistObject (_order);

      Assert.That (_manager.GetEnlistedObject (_order.ID), Is.SameAs (_order));
    }

    [Test]
    public void GetEnlistedDomainObject_NotEnlisted ()
    {
      Assert.That (_manager.GetEnlistedObject (_order.ID), Is.Null);
    }
  }
}