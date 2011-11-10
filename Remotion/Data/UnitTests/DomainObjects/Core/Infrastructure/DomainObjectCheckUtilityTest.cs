// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectCheckUtilityTest : ClientTransactionBaseTest
  {
    [Test]
    public void EnsureNotInvalid_Valid ()
    {
      var order = Order.NewObject ();

      DomainObjectCheckUtility.EnsureNotInvalid (order, ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void EnsureNotInvalid_Discarded ()
    {
      var order = Order.NewObject ();
      order.Delete ();
      DomainObjectCheckUtility.EnsureNotInvalid (order, ClientTransaction.Current);
    }

    [Test]
    public void CheckIfRightTransaction_Works ()
    {
      var order = Order.NewObject ();

      DomainObjectCheckUtility.CheckIfRightTransaction (order, ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object 'Order|.*|System.Guid' cannot be used in the "
        + "given transaction as it was loaded or created in another transaction. Enter a scope for the transaction, or call EnlistInTransaction to "
        + "enlist the object in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
          MatchType = MessageMatch.Regex)]
    public void CheckIfRightTransaction_Fails ()
    {
      var order = Order.NewObject ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectCheckUtility.CheckIfRightTransaction (order, ClientTransaction.Current);
      }
    }

    [Test]
    public void EnsureNotDeleted_NotDeleted ()
    {
      var relatedObject = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      DomainObjectCheckUtility.EnsureNotDeleted (relatedObject, ClientTransactionMock);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void EnsureNotDeleted_Deleted ()
    {
      var relatedObject = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      relatedObject.Delete ();

      DomainObjectCheckUtility.EnsureNotDeleted (relatedObject, ClientTransactionMock);
    }
  }
}
