using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> 
///   Throw this exception to cancel the execution of a <see cref="WxeFunction"/> while executing a 
///   <see cref="WxePageStep"/>. 
/// </summary>
[Serializable]
public class WxeUserCancelException: WxeException
{
  public WxeUserCancelException()
    : this ("User cancelled this step.")
  {
  }

  public WxeUserCancelException(string message)
    : base (message)
  {
  }
  public WxeUserCancelException(string message, Exception innerException)
    : base (message, innerException)
  {
  }

  protected WxeUserCancelException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
