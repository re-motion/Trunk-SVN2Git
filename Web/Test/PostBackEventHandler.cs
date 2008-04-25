using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.Test
{
  public class PostBackEventHandler : WebControl, IPostBackEventHandler
  {
    public event EventHandler<IDEventArgs> PostBack;

    public void RaisePostBackEvent (string eventArgument)
    {
      if (PostBack != null)
        PostBack (this, new IDEventArgs (eventArgument));
    }
  }
}