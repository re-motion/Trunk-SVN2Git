using System;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  [Serializable]
  public class NoAutoCommitTestTransactedFunction: WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields
    private WxeTransactionMode _transactionMode;

    // construction and disposing

    public NoAutoCommitTestTransactedFunction (WxeTransactionMode transactionMode, ObjectID objectWithAllDataTypes)
        : base (transactionMode, objectWithAllDataTypes)
    {
      _transactionMode = transactionMode;
    }

    // methods and properties

    protected override bool AutoCommit
    {
      get { return false; }
    }

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ObjectID ObjectWithAllDataTypes
    {
      get { return (ObjectID) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1()
    {
      ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (ObjectWithAllDataTypes);

      objectWithAllDataTypes.Int32Property = 10;
    }
  }
}