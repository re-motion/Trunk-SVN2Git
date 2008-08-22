using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{
  public abstract class WxeExecutionControlException : WxeException
  {
    public WxeExecutionControlException(string message)
        : base (message)
    {
    }

    protected WxeExecutionControlException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}