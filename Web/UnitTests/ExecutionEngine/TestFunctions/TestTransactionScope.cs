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

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestTransactionScope : ITransactionScope<TestTransaction>
  {
    private static TestTransactionScope _currentScope;

    private readonly TestTransaction _scopedTransaction;
    private readonly TestTransactionScope _previousScope;
    private bool _left = false;

    public TestTransactionScope (TestTransaction scopedTransaction)
    {
      _scopedTransaction = scopedTransaction;
      _previousScope = _currentScope;
      CurrentScope = this;
    }

    public TestTransaction ScopedTransaction
    {
      get { return _scopedTransaction; }
    }

    public static TestTransactionScope CurrentScope
    {
      get { return _currentScope; }
      set
      {
        _currentScope = value;
        TestTransaction.Current = value != null ? value.ScopedTransaction : null;
      }
    }

    public void Leave ()
    {
      if (_left)
        throw new InvalidOperationException ("Has already been left.");
      CurrentScope = _previousScope;
      _left = true;
    }
  }
}