using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
[Serializable]
public class CreateRootTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateRootTestTransactedFunction (ClientTransactionScope previousClientTransactionScope) 
      : base (WxeTransactionMode.CreateRoot, previousClientTransactionScope)
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
    Assert.AreNotSame (PreviousClientTransactionScope, ClientTransactionScope.ActiveScope);
  }

}
}
