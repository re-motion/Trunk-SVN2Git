using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class DomainObjectParameterInvalidOutTestTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectParameterInvalidOutTestTransactedFunction (WxeTransactionMode transactionMode)
        : base (transactionMode)
    {
    }

    // methods and properties

    [WxeParameter (1, false, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes OutParameter
    {
      get { return (ClassWithAllDataTypes) Variables["OutParameter"]; }
      set { Variables["OutParameter"] = value; }
    }

    private void Step1 ()
    {
      OutParameter = ClassWithAllDataTypes.NewObject();
      OutParameter.Delete();
    }
  }
}