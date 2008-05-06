using System;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class CreateNoneTestTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateNoneTestTransactedFunction (ClientTransaction previousClientTransaction)
        : base (WxeTransactionMode.None, previousClientTransaction)
    {
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ClientTransaction PreviousClientTransaction
    {
      get { return (ClientTransaction) Variables["PreviousClientTransaction"]; }
      set { Variables["PreviousClientTransaction"] = value; }
    }

    private void Step1 ()
    {
      if (ClientTransactionScope.CurrentTransaction != PreviousClientTransaction)
        throw new TestFailureException ("The WxeTransactedFunction of the parent function was not properly used.");
    }
  }
}