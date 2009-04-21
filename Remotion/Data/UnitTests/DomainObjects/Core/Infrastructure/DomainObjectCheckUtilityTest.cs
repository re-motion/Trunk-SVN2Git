// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectCheckUtilityTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetNonNullClientTransaction_Current()
    {
      var order = Order.NewObject();
      Assert.That (DomainObjectCheckUtility.GetNonNullClientTransaction (order), Is.SameAs (ClientTransaction.Current));
    }

    [Test]
    public void GetNonNullClientTransaction_Bound ()
    {
      Order order;
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        order = Order.NewObject();
      }
      Assert.That (DomainObjectCheckUtility.GetNonNullClientTransaction (order), Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void GetNonNullClientTransaction_Null ()
    {
      var order = Order.NewObject ();
      using (ClientTransactionScope.EnterNullScope ())
      {
        DomainObjectCheckUtility.GetNonNullClientTransaction (order);
      }
    }

    [Test]
    public void CheckIfObjectIsDiscarded_Valid ()
    {
      var order = Order.NewObject ();
      DomainObjectCheckUtility.CheckIfObjectIsDiscarded (order, ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void CheckIfObjectIsDiscarded_Discarded ()
    {
      var order = Order.NewObject ();
      order.Delete ();
      DomainObjectCheckUtility.CheckIfObjectIsDiscarded (order, ClientTransaction.Current);
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
  }
}
