using System;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [WxeDemandTargetStaticMethodPermission ("Search", typeof (SecurableObject))]
  public class TestFunctionWithPermissionsFromStaticMethod : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithPermissionsFromStaticMethod ()
      : base ()
    {
    }

    // methods and properties
  }
}