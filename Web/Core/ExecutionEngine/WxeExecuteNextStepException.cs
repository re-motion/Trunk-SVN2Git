using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> This exception is used by the execution engine to end the execution of a <see cref="WxePageStep"/>. </summary>
[Serializable]
public class WxeExecuteNextStepException: WxeException
{
  public WxeExecuteNextStepException()
    : base ("This exception does not indicate an error. It is used to roll back the call stack. It is recommended to disable breaking on this exeption type while debugging.")
  {
  }

  protected WxeExecuteNextStepException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
