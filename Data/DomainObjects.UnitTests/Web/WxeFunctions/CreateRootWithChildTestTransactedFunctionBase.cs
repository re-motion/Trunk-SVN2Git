using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  public class CreateRootWithChildTestTransactedFunctionBase : WxeTransactedFunction
  {
    public WxeFunction ChildFunction;

    public CreateRootWithChildTestTransactedFunctionBase (WxeTransactionMode mode, WxeFunction childFunction, params object[] actualParameters)
        : base(mode, actualParameters)
    {
      Add (childFunction);
      ChildFunction = childFunction;
    }
  }
}