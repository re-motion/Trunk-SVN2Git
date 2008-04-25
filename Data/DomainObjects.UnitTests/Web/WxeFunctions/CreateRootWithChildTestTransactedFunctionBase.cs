using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
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