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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void HasBindingTransaction_BoundObject ()
    {
      using (ClientTransaction.CreateBindingTransaction ().EnterDiscardingScope ())
      {
        var obj = Order.NewObject ();
        Assert.That (obj.HasBindingTransaction, Is.True);
      }
    }

    [Test]
    public void HasBindingTransaction_UnboundObject ()
    {
      var obj = Order.NewObject ();
      Assert.That (obj.HasBindingTransaction, Is.False);
    }

    [Test]
    public void GetBindingTransaction_BoundObject ()
    {
      using (ClientTransaction.CreateBindingTransaction ().EnterDiscardingScope ())
      {
        var obj = Order.NewObject ();
        Assert.That (obj.GetBindingTransaction(), Is.SameAs (ClientTransaction.Current));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This object has not been bound to a specific transaction, it "
        + "uses the current transaction when it is accessed. Use HasBindingTransaction to check whether an object has been bound or not.")]
    public void GetBindingTransaction_UnboundObject ()
    {
      var obj = Order.NewObject ();
      Dev.Null = obj.GetBindingTransaction();
    }
  }
}