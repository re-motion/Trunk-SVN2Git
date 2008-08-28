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
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestableWxeScopedTransaction : WxeScopedTransaction<TestTransaction, TestTransactionScope, TestTransactionScopeManager>
  {
    public TestableWxeScopedTransaction (WxeStepList steps, bool autoCommit, bool forceRoot)
        : base (steps, autoCommit, forceRoot)
    {
    }

    public TestableWxeScopedTransaction (bool autoCommit, bool forceRoot)
        : base (autoCommit, forceRoot)
    {
    }

    public TestableWxeScopedTransaction (bool autoCommit)
        : base (autoCommit)
    {
    }

    public TestableWxeScopedTransaction ()
    {
    }

    public new bool AutoCommit
    {
      get { return base.AutoCommit; }
    }

    public new bool ForceRoot
    {
      get { return base.ForceRoot; }
    }

    public new TestTransaction CurrentTransaction
    {
      get { return base.CurrentTransaction; }
    }

    public new void CheckCurrentTransactionResettable ()
    {
      base.CheckCurrentTransactionResettable ();
    }

    public new void SetCurrentTransaction (TestTransaction transaction)
    {
      base.SetCurrentTransaction (transaction);
    }

    public new void SetPreviousCurrentTransaction (TestTransaction previousTransaction)
    {
      base.SetPreviousCurrentTransaction (previousTransaction);
    }
  }
}