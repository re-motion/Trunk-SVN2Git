using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestableScopedTransactedFunction : WxeScopedTransactedFunction<TestTransaction, TestTransactionScope, TestTransactionScopeManager>
  {
    private bool _autoCommit;

    public TestableScopedTransactedFunction (params object[] actualParameters)
      : base (actualParameters)
    {
      _autoCommit = base.AutoCommit;
    }

    public TestableScopedTransactedFunction (WxeTransactionMode transactionMode, params object[] actualParameters)
      : base (transactionMode, actualParameters)
    {
      _autoCommit = base.AutoCommit;
    }

    public override WxeParameterDeclaration[] ParameterDeclarations
    {
      get 
      { 
        return new WxeParameterDeclaration[] { 
          new WxeParameterDeclaration ("in", false, WxeParameterDirection.In, typeof (object)),
          new WxeParameterDeclaration ("out", false, WxeParameterDirection.Out, typeof (object)),
          new WxeParameterDeclaration ("inout", false, WxeParameterDirection.InOut, typeof (object))
      }; }
    }

    public new WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return base.CreateWxeTransaction ();
    }

    protected override bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public bool InternalAutoCommit
    {
      get { return _autoCommit; }
      set { _autoCommit = value; }
    }

    public new TestTransaction CreateRootTransaction ()
    {
      return base.CreateRootTransaction ();
    }

    public new WxeScopedTransaction<TestTransaction, TestTransactionScope, TestTransactionScopeManager> CreateWxeTransaction (bool autoCommit, bool forceRoot)
    {
      return base.CreateWxeTransaction (autoCommit, forceRoot);
    }

    public new void OnTransactionCreated (TestTransaction transaction)
    {
      base.OnTransactionCreated (transaction);
    }

    public new void OnExecutionFinished ()
    {
      base.OnExecutionFinished ();
    }
  }
}