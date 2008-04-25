using System;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [WxeDemandCreatePermission (typeof (SecurableObject))]
  public class TestFunctionWithPermissionsFromConstructor : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithPermissionsFromConstructor ()
      : base ()
    {
    }

    // methods and properties
  }
}