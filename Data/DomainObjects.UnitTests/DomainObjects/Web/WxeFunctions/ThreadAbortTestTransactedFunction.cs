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
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  using WxeTransactedFunction =
      Remotion.Web.ExecutionEngine.WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class ThreadAbortTestTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ThreadAbortTestTransactedFunction ()
        : base()
    {
    }

    public bool FirstStepExecuted;
    public bool SecondStepExecuted;
    public bool ThreadAborted;

    public ClientTransactionScope TransactionScopeInFirstStep;
    public ClientTransactionScope TransactionScopeInSecondStepBeforeException;
    public ClientTransactionScope TransactionScopeInSecondStepAfterException;

    // methods and properties

    private void Step1 ()
    {
      Assert.IsFalse (FirstStepExecuted);
      Assert.IsFalse (SecondStepExecuted);
      Assert.IsFalse (ThreadAborted);
      FirstStepExecuted = true;
      TransactionScopeInFirstStep = ClientTransactionScope.ActiveScope;
    }

    private void Step2 ()
    {
      Assert.IsTrue (FirstStepExecuted);
      Assert.IsFalse (SecondStepExecuted);

      if (!ThreadAborted)
      {
        TransactionScopeInSecondStepBeforeException = ClientTransactionScope.ActiveScope;
        Assert.AreSame (TransactionScopeInFirstStep, TransactionScopeInSecondStepBeforeException);
        ThreadAborted = true;
        Thread.CurrentThread.Abort();
      }
      TransactionScopeInSecondStepAfterException = ClientTransactionScope.ActiveScope;
      Assert.AreNotSame (TransactionScopeInFirstStep, TransactionScopeInSecondStepAfterException);
      Assert.AreSame (TransactionScopeInFirstStep.ScopedTransaction, TransactionScopeInSecondStepAfterException.ScopedTransaction);
      SecondStepExecuted = true;
    }
  }
}
