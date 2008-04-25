using System;
using System.Threading;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.UnitTesting;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  [Serializable]
  public class ThreadAbortTestTransactedFunction: WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ThreadAbortTestTransactedFunction ()
        : base ()
    {
    }

    public bool FirstStepExecuted;
    public bool SecondStepExecuted;
    public bool ThreadAborted;

    public ClientTransactionScope TransactionScopeInFirstStep;
    public ClientTransactionScope TransactionScopeInSecondStepBeforeException;
    public ClientTransactionScope TransactionScopeInSecondStepAfterException;

    // methods and properties

    private void Step1()
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