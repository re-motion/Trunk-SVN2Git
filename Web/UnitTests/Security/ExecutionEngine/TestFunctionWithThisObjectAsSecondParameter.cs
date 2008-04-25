using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  public class TestFunctionWithThisObjectAsSecondParameter : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithThisObjectAsSecondParameter (object someObject, SecurableObject thisObject)
      : base (someObject, thisObject)
    {
    }

    // methods and properties

    [WxeParameter (0, true, WxeParameterDirection.In)]
    public object SomeObject
    {
      get
      {
        return Variables["SomeObject"];
      }
      set
      {
        Variables["SomeObject"] = value;
      }
    }

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public SecurableObject ThisObject
    {
      get
      {
        return (SecurableObject) Variables["ThisObject"];
      }
      set
      {
        Variables["ThisObject"] = value;
      }
    }
  }
}