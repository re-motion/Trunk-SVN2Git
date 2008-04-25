using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
[Serializable]
public class RemoveCurrentTransactionScopeFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RemoveCurrentTransactionScopeFunction () 
      : base (WxeTransactionMode.CreateRoot)
  {
  }

  // methods and properties

  private void Step1 ()
  {
    ClientTransactionScope.ActiveScope.Leave ();
  }

}
}
