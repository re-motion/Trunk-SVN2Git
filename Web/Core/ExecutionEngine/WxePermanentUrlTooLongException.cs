using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary>
///   This exception occurs when a permanent URL would exceed the maximum URL length.
/// </summary>
[Serializable]
public class WxePermanentUrlTooLongException: WxeException
{
	public WxePermanentUrlTooLongException (string message)
    : base (message)
	{
  }

  public WxePermanentUrlTooLongException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
