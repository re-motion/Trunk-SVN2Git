using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary>
/// This exception occurs when a WxeFunction constructor with one or more <c>out</c> parameters is called.
/// </summary>
[Serializable]
public class WxeOutParameterNotSupportedException: NotSupportedException
{
  public new const string Message = "Cannot use WxeFunction constructors with one or more 'out' parameters. These constructors are provided for intellisense only.";

	public WxeOutParameterNotSupportedException()
    : base (Message)
	{
  }

  public WxeOutParameterNotSupportedException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
