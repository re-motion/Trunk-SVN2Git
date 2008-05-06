using System;
using System.Threading;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
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