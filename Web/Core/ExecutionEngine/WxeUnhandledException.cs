using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   Encapsulates an exception when it is re-thrown by a <see cref="WxeFunction"/>.
  /// </summary>
  [Serializable]
  public class WxeUnhandledException : WxeException
  {

    public WxeUnhandledException (string message)
      : base (message)
    {
    }

    public WxeUnhandledException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    public WxeUnhandledException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }

}
