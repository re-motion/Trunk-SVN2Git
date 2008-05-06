using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class DomainObjectParameterWithChildInvalidOutTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildInvalidOutTestTransactedFunction ()
        : base (WxeTransactionMode.CreateRoot, new DomainObjectParameterInvalidOutTestTransactedFunction (WxeTransactionMode.CreateChildIfParent))
    {
    }
  }
}
