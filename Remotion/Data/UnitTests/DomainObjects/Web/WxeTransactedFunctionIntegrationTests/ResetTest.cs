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
using Remotion.Data.DomainObjects.Persistence;
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
    public void Reset_Variables_AreEnlistedInNewTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = Order.GetObject (DomainObjectIDs.Order1);
        f.Variables.Add ("Order", order);

        f.Transaction.Reset ();

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction> ();
        Assert.That (transactionAfter.IsEnlisted (order), Is.True);
      });
    }

    [Test]
    public void Reset_NonLoadableVariables ()
    {
      SetDatabaseModifyable();

      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var existingObject = Order.GetObject (DomainObjectIDs.Order1);
        var nonExistingObject = Employee.GetObject (DomainObjectIDs.Employee1);

        ClientTransaction.CreateRootTransaction ().Execute (() =>
        {
          Employee.GetObject (nonExistingObject.ID).Delete();
          ClientTransaction.Current.Commit();
        });

        ClientTransaction.Current.Rollback();

        f.Variables.Add ("ExistingObject", existingObject);
        f.Variables.Add ("NonExistingObject", nonExistingObject);

        f.Transaction.Reset ();

        Assert.That (existingObject.State, Is.EqualTo (StateType.NotLoadedYet));
        Assert.That (nonExistingObject.State, Is.EqualTo (StateType.NotLoadedYet));

        Assert.That (() => existingObject.EnsureDataAvailable (), Throws.Nothing);
        Assert.That (() => nonExistingObject.EnsureDataAvailable (), Throws.TypeOf<ObjectsNotFoundException> ());
      });
    }

    [Test]
    public void Reset_NonVariables_AreNotEnlistedInNewTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = Order.GetObject (DomainObjectIDs.Order1);

        f.Transaction.Reset ();

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

        var order = Order.GetObject (DomainObjectIDs.Order1);

        f.Transaction.Reset();

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction>();
        Assert.That (transactionAfter, Is.Not.SameAs (transactionBefore));
        Assert.That (transactionAfter, Is.Not.Null.And.SameAs (ClientTransaction.Current));
        Assert.That (transactionAfter.ParentTransaction, Is.Not.Null.And.SameAs (parentBefore));

        // This is because it was automatically enlisted in the outer transaction before the reset
        Assert.That (transactionAfter.IsEnlisted (order), Is.True);
      });
    }

    [Test]
    public void Reset_InvalidVariables_AndOtherObjects_AreNotKeptInvalid ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var invalidVariable = Order.NewObject();
        invalidVariable.Delete();
        f.Variables.Add ("InvalidOrderVariable", invalidVariable);
        Assert.That (invalidVariable.IsInvalid, Is.True);

        var invalidNonVariable = Order.NewObject();
        invalidNonVariable.Delete ();
        Assert.That (invalidNonVariable.IsInvalid, Is.True);

        f.Transaction.Reset ();

        Assert.That (ClientTransaction.Current.IsEnlisted (invalidVariable), Is.True);
        Assert.That (invalidVariable.IsInvalid, Is.False);

        Assert.That (ClientTransaction.Current.IsEnlisted (invalidNonVariable), Is.False);
      });
    }
  }
}