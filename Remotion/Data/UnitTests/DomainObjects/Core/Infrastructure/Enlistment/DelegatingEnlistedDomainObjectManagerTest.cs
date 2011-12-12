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
  public class DelegatingEnlistedDomainObjectManagerTest : StandardMappingTest
  {
    private ClientTransaction _otherTransaction;
    private DelegatingEnlistedDomainObjectManager _manager;
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp ();

      _otherTransaction = ClientTransaction.CreateRootTransaction ();
      var targetManager = ClientTransactionTestHelper.GetEnlistedDomainObjectManager (_otherTransaction);
      _manager = new DelegatingEnlistedDomainObjectManager (targetManager);
      _order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
    }

    [Test]
    public void IsEnlisted_Delegates ()
    {
      Assert.That (_manager.IsEnlisted (_order), Is.False);

      _otherTransaction.EnlistDomainObject (_order);

      Assert.That (_manager.IsEnlisted (_order), Is.True);
    }

    [Test]
    public void EnlistDomainObject_Delegates ()
    {
      Assert.That (_otherTransaction.IsEnlisted (_order), Is.False);

      _manager.EnlistDomainObject (_order);

      Assert.That (_otherTransaction.IsEnlisted (_order), Is.True);
    }

    [Test]
    public void EnlistedDomainObjectCount_Delegates ()
    {
      Assert.That (_manager.EnlistedDomainObjectCount, Is.EqualTo (0));

      _otherTransaction.EnlistDomainObject (_order);

      Assert.That (_manager.EnlistedDomainObjectCount, Is.EqualTo (1));
    }

    [Test]
    public void GetEnlistedDomainObjects_Delegates ()
    {
      _otherTransaction.EnlistDomainObject (_order);

      Assert.That (_manager.GetEnlistedDomainObjects ().ToArray(), Is.EqualTo (new[] { _order }));
    }

    [Test]
    public void GetEnlistedDomainObject_Delegates ()
    {
      _otherTransaction.EnlistDomainObject (_order);

      Assert.That (_manager.GetEnlistedDomainObject (_order.ID), Is.SameAs (_order));
    }

    [Test]
    public void GetEnlistedDomainObject_NotEnlisted ()
    {
      Assert.That (_manager.GetEnlistedDomainObject (_order.ID), Is.Null);
    }
  }
}