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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class ResetTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void Reset_ReplacesCurrentTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));

        f.Transaction.Reset ();

        Assert.That (transactionBefore, Is.Not.SameAs (ClientTransaction.Current));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));
      });
    }

    [Test]
    public void Reset_VariablesOfDomainObjectTypes_CauseException ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = DomainObjectIDs.Order1.GetObject<Order> ();
        f.Variables.Add ("Order", order);
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction>()));

        Assert.That (
            () =>
            f.Transaction.Reset(),
            Throws.TypeOf<WxeException>().With.Message.EqualTo (
                "One or more of the variables of the WxeFunction are incompatible with the new transaction after the Reset. The following objects "
                + "are incompatible with the target transaction: Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid. "
                + "Use variables of type 'Remotion.Data.DomainObjects.IDomainObjectHandle`1[T]' instead."));

        Assert.That (ClientTransaction.Current, Is.Not.Null.And.Not.SameAs (transactionBefore));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction>()));
      });
    }

    [Test]
    public void Reset_VariablesOfDomainObjectHandleType_CauseNoException ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var orderHandle = DomainObjectIDs.Order1.GetHandle<Order> ();
        f.Variables.Add ("OrderHandle", orderHandle);
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));

        Assert.That (() => f.Transaction.Reset (), Throws.Nothing);

        Assert.That (ClientTransaction.Current, Is.Not.Null.And.Not.SameAs (transactionBefore));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));
      });
    }

    [Test]
    public void Reset_NonVariables_CauseNoException ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = DomainObjectIDs.Order1.GetObject<Order> ();

        Assert.That (() => f.Transaction.Reset (), Throws.Nothing);

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction> ();
        Assert.That (transactionAfter.IsEnlisted (order), Is.False);
      });
    }

    [Test]
    public void Reset_WithChildTransaction ()
    {
      ExecuteDelegateInSubWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent, (ctx, f) =>
      {
        var transactionBefore = f.Transaction.GetNativeTransaction<ClientTransaction>();
        Assert.That (transactionBefore, Is.Not.Null.And.SameAs (ClientTransaction.Current));
        Assert.That (transactionBefore.ParentTransaction, Is.Not.Null);
        var parentBefore = transactionBefore.ParentTransaction;

        var order = DomainObjectIDs.Order1.GetObject<Order> ();
        f.Variables.Add ("Order", order);

        f.Transaction.Reset();

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction>();
        Assert.That (transactionAfter, Is.Not.SameAs (transactionBefore));
        Assert.That (transactionAfter, Is.Not.Null.And.SameAs (ClientTransaction.Current));
        Assert.That (transactionAfter.ParentTransaction, Is.Not.Null.And.SameAs (parentBefore));

        // This is because it was automatically enlisted in the outer transaction before the reset
        Assert.That (transactionAfter.IsEnlisted (order), Is.True);
      });
    }
  }
}