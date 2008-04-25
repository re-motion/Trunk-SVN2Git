using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> This exception is thrown by the execution engine. </summary>
[Serializable]
public class WxeException: ApplicationException
{
	public WxeException()
    : base ("The execution engine encountered an error.")
	{
  }

  public WxeException (string message)
    : base (message)
  {
  }

  public WxeException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  public WxeException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}
}
