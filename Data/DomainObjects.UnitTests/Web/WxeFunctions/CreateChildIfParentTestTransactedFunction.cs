using System;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class CreateChildIfParentTestTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateChildIfParentTestTransactedFunction ()
        : base (WxeTransactionMode.CreateChildIfParent)
    {
    }

    // methods and properties

    private void Step1 ()
    {
      ITransaction parentTransaction = ((WxeTransactedFunction)ParentFunction).MyTransaction;
      Assert.AreNotSame (parentTransaction, ClientTransactionScope.CurrentTransaction);
      Assert.AreSame (parentTransaction, ClientTransactionScope.CurrentTransaction.ParentTransaction);
    }
  }
}