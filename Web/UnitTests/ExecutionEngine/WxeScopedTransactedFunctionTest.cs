/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  using NUnit.Framework.SyntaxHelpers;

  [TestFixture]
  public class WxeScopedTransactedFunctionTest : WxeTest
  {
    private TestableScopedTransactedFunction _function;

    public override void SetUp ()
    {
      base.SetUp ();
      _function = new TestableScopedTransactedFunction ();
      TestTransactionScope.CurrentScope = null;
    }

    public override void TearDown ()
    {
      TestTransactionScope.CurrentScope = null;
      base.TearDown ();
    }

    [Test]
    public void DefaultTransactionMode ()
    {
      Assert.That (_function.TransactionMode, Is.EqualTo (WxeTransactionMode.CreateRoot));
    }

    [Test]
    public void SetTransactionMode ()
    {
      Assert.That (_function.TransactionMode, Is.EqualTo (WxeTransactionMode.CreateRoot));
      _function.TransactionMode = WxeTransactionMode.None;
      Assert.That (_function.TransactionMode, Is.EqualTo (WxeTransactionMode.None));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "TransactionMode must not be set after execution of this function has " 
        + "started.")]
    public void SetTransactionMode_AfterExecutionStarted ()
    {
      _function.Add (new WxeDelegateStep (delegate { _function.TransactionMode = WxeTransactionMode.None; }));
      try
      {
        _function.Execute (CurrentWxeContext);
      }
      catch (WxeUnhandledException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void AutoCommit_Default ()
    {
      Assert.That (_function.InternalAutoCommit, Is.True);
    }

    [Test]
    public void AutoCommit_Overridden ()
    {
      _function.InternalAutoCommit = false;
      Assert.That (_function.InternalAutoCommit, Is.False);
    }

    [Test]
    public void CreateWxeTransaction_None ()
    {
      _function.TransactionMode = WxeTransactionMode.None;
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction ();
      Assert.That (transaction, Is.Null);
    }

    [Test]
    public void CreateWxeTransaction_CreateRoot ()
    {
      _function.TransactionMode = WxeTransactionMode.CreateRoot;
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction ();
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "ForceRoot"), Is.True);
    }

    [Test]
    public void CreateWxeTransaction_CreateChildIfParent ()
    {
      _function.TransactionMode = WxeTransactionMode.CreateChildIfParent;
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction ();
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "ForceRoot"), Is.False);
    }

    [Test]
    public void CreateWxeTransaction_AutoCommit ()
    {
      _function.InternalAutoCommit = true;
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction ();
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "AutoCommit"), Is.True);
    }

    [Test]
    public void CreateWxeTransaction_AutoCommit_False ()
    {
      _function.InternalAutoCommit = false;
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction ();
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "AutoCommit"), Is.False);
    }

    [Test]
    public void CreateWxeTransaction_Implementation_AutoCommit_ForceRoot ()
    {
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction (true, true);
      Assert.That (transaction, Is.InstanceOfType (typeof (WxeScopedTransaction<TestTransaction, TestTransactionScope, TestTransactionScopeManager>)));
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "AutoCommit"), Is.True);
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "ForceRoot"), Is.True);
    }

    [Test]
    public void CreateWxeTransaction_Implementation_AutoCommit_False_ForceRoot_False ()
    {
      WxeTransactionBase<TestTransaction> transaction = _function.CreateWxeTransaction (false, false);
      Assert.That (transaction, Is.InstanceOfType (typeof (WxeScopedTransaction<TestTransaction, TestTransactionScope, TestTransactionScopeManager>)));
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "AutoCommit"), Is.False);
      Assert.That (PrivateInvoke.GetNonPublicProperty (transaction, "ForceRoot"), Is.False);
    }

    [Test]
    public void CreateRootTransaction ()
    {
      TestTransaction transaction = _function.CreateRootTransaction ();
      Assert.That (transaction, Is.SameAs (GetScopeManager().RootTransactionToCreate)) ;
    }

    [Test]
    public void OnTransactionCreated_EnlistsInParameters ()
    {
      _function.Variables["in"] = "i";
      _function.Variables["inout"] = "io";
      _function.Variables["out"] = "o";

      TestTransaction transaction = GetScopeManager ().RootTransactionToCreate;
      _function.OnTransactionCreated (transaction);

      Assert.That (GetScopeManager ().EnlistedObjects.ContainsKey (transaction));
      Assert.That (GetScopeManager ().EnlistedObjects[transaction], Is.EquivalentTo (new object[] { "i", "io" }));
    }

    [Test]
    public void OnTransactionCreated_LoadsInParameters ()
    {
      _function.Variables["in"] = "i";
      _function.Variables["inout"] = "io";
      _function.Variables["out"] = "o";

      TestTransaction transaction = GetScopeManager ().RootTransactionToCreate;
      _function.OnTransactionCreated (transaction);

      Assert.That (GetScopeManager ().LoadedObjects.ContainsKey (transaction));
      Assert.That (GetScopeManager ().LoadedObjects[transaction], Is.EquivalentTo (new object[] { "i", "io" }));
    }

    [Test]
    public void OnExecuteFinished_EnlistsOutParameters_WithScopedTransaction ()
    {
      TestTransaction transaction = new TestTransaction ();

      _function.Variables["in"] = "i";
      _function.Variables["inout"] = "io";
      _function.Variables["out"] = "o";

      new TestTransactionScope (transaction);
      _function.OnExecutionFinished ();

      Assert.That (GetScopeManager().EnlistedObjects.ContainsKey (transaction));
      Assert.That (GetScopeManager().EnlistedObjects[transaction], Is.EquivalentTo (new object[] {"o", "io"}));
   }

    [Test]
    public void OnExecuteFinished_LoadsOutParameters_WithScopedTransaction ()
    {
      TestTransaction transaction = new TestTransaction ();

      _function.Variables["in"] = "i";
      _function.Variables["inout"] = "io";
      _function.Variables["out"] = "o";

      new TestTransactionScope (transaction);
      _function.OnExecutionFinished ();

      Assert.That (GetScopeManager ().LoadedObjects.ContainsKey (transaction));
      Assert.That (GetScopeManager ().LoadedObjects[transaction], Is.EquivalentTo (new object[] { "o", "io" }));
    }

    [Test]
    public void OnExecuteFinished_DoesntHandleOutParameters_WithNullTransaction ()
    {
      _function.Variables["in"] = "i";
      _function.Variables["inout"] = "io";
      _function.Variables["out"] = "o";

      new TestTransactionScope (null);
      _function.OnExecutionFinished ();

      Assert.That (GetScopeManager ().EnlistedObjects.Count, Is.EqualTo (0));
      Assert.That (GetScopeManager ().LoadedObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void OnExecuteFinished_DoesntHandleOutParameters_WithoutOuterScope ()
    {
      _function.Variables["in"] = "i";
      _function.Variables["inout"] = "io";
      _function.Variables["out"] = "o";

      _function.OnExecutionFinished ();

      Assert.That (GetScopeManager ().EnlistedObjects.Count, Is.EqualTo (0));
      Assert.That (GetScopeManager ().LoadedObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void ResetTransaction_Copy ()
    {
      TestTransaction transaction1 = GetScopeManager ().RootTransactionToCreate;
      TestTransaction transaction2 = new TestTransaction();

      _function.Add (new WxeDelegateStep (delegate
          {
            GetScopeManager().RootTransactionToCreate = transaction2;
            _function.ResetTransaction (true);
          }));

      _function.Execute ();

      Assert.That (GetScopeManager ().EnlistedSameObjects.ContainsKey (transaction2));
      Assert.That (GetScopeManager ().EnlistedSameObjects[transaction2], Is.EqualTo (Tuple.NewTuple (transaction1, true)));

      Assert.That (GetScopeManager ().CopiedTransactionEventHandlers.ContainsKey (transaction2));
      Assert.That (GetScopeManager ().CopiedTransactionEventHandlers[transaction2], Is.EqualTo (transaction1));
    }

    [Test]
    public void ResetTransaction_NoCopy ()
    {
      TestTransaction transaction1 = GetScopeManager ().RootTransactionToCreate;
      TestTransaction transaction2 = new TestTransaction ();

      _function.Add (new WxeDelegateStep (delegate
          {
            GetScopeManager ().RootTransactionToCreate = transaction2;
            _function.ResetTransaction (false);
          }));

      _function.Execute ();

      Assert.That (GetScopeManager ().EnlistedSameObjects.ContainsKey (transaction2));
      Assert.That (GetScopeManager ().EnlistedSameObjects[transaction2], Is.EqualTo (Tuple.NewTuple (transaction1, false)));

      Assert.That (GetScopeManager ().CopiedTransactionEventHandlers, Is.Empty);
    }

    [Test]
    public void ResetTransaction_Default ()
    {
      TestTransaction transaction1 = GetScopeManager ().RootTransactionToCreate;
      TestTransaction transaction2 = new TestTransaction ();

      _function.Add (new WxeDelegateStep (delegate
          {
            GetScopeManager ().RootTransactionToCreate = transaction2;
            _function.ResetTransaction ();
          }));

      _function.Execute ();

      Assert.That (GetScopeManager ().EnlistedSameObjects.ContainsKey (transaction2));
      Assert.That (GetScopeManager ().EnlistedSameObjects[transaction2], Is.EqualTo (Tuple.NewTuple (transaction1, false)));

      Assert.That (GetScopeManager ().CopiedTransactionEventHandlers, Is.Empty);
    }

    [Test]
    public void SerializingAndAbortingFunction_AfterConmitThrows_LeavesConsistentState ()
    {
      _function.Add (new WxeDelegateStep (delegate { TestTransaction.Current.ThrowOnCommit = true; }));
      try
      {
        _function.Execute ();
        Assert.Fail ("Expected CommitException");
      }
      catch (WxeUnhandledException ex)
      {
        Assert.That (ex.InnerException, Is.InstanceOfType (typeof (CommitException)));
      }
      var deserializedFunction = Serializer.SerializeAndDeserialize (_function);
      deserializedFunction.Abort ();
      Assert.That (deserializedFunction.IsAborted, Is.True);
    }

    private TestTransactionScopeManager GetScopeManager ()
    {
      return (TestTransactionScopeManager) PrivateInvoke.GetNonPublicProperty (_function, "ScopeManager");
    }
  }
}
