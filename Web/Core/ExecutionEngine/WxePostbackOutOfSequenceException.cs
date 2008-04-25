using System;
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> This exception indicates an attempt to resubmit a page cached in the browser's history. </summary>
[Serializable]
public class WxePostbackOutOfSequenceException: WxeException
{
  public WxePostbackOutOfSequenceException()
    : base ("The server has received a post back from a page that has already been submitted. "
        + "The page's state is no longer valid. Please navigate to the start page to restart the web application.")
  {
  }

  protected WxePostbackOutOfSequenceException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
