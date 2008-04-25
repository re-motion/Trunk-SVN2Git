using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

public class TestFunctionWithNonSerializableParameters: WxeFunction
{
  public TestFunctionWithNonSerializableParameters()
	{
	}

	public TestFunctionWithNonSerializableParameters (params object[] args)
    : base (args)
	{
	}

  public TestFunctionWithNonSerializableParameters (string StringValue, object ObjectValue)
    : base (StringValue, ObjectValue)
  {
  }

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public string StringValue
  {
    get { return (string) Variables["StringValue"]; }
    set { Variables["StringValue"] = value; }
  }

  [WxeParameter (2, true, WxeParameterDirection.In)]
  public object ObjectValue
  {
    get { return Variables["ObjectValue"]; }
    set { Variables["ObjectValue"] = value; }
  }
}

}
