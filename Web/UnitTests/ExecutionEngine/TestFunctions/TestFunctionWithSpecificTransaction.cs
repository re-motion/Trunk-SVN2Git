/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestFunctionWithSpecificTransaction : WxeTransactedFunctionBase<TestTransaction>
  {
    private readonly TestTransaction _transaction;
    
    public TestTransaction TransactionInFirstStep;
    public TestWxeTransaction WxeTransaction;

    public TestFunctionWithSpecificTransaction (TestTransaction transaction)
    {
      _transaction = transaction;
    }

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      WxeTransaction = new TestWxeTransaction ();
      return WxeTransaction;
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      return _transaction;
    }

    private void Step1 ()
    {
      TransactionInFirstStep = TestTransaction.Current;
    }
  }
}