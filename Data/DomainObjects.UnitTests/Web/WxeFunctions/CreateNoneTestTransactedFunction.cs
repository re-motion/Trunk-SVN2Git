using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class CreateNoneTestTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateNoneTestTransactedFunction (ClientTransactionScope previousClientTransactionScope)
        : base (WxeTransactionMode.None, previousClientTransactionScope)
    {
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ClientTransactionScope PreviousClientTransactionScope
    {
      get { return (ClientTransactionScope) Variables["PreviousClientTransactionScope"]; }
      set { Variables["PreviousClientTransactionScope"] = value; }
    }

    private void Step1 ()
    {
      Assert.AreSame (PreviousClientTransactionScope, ClientTransactionScope.ActiveScope);
    }
  }
}