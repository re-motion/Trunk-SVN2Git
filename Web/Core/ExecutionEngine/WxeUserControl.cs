using System;
using System.Web.UI;
using Remotion.Collections;

namespace Remotion.Web.ExecutionEngine
{

public class WxeUserControl: UserControl, IWxeTemplateControl
{
  WxeTemplateControlInfo _wxeInfo;

  public WxeUserControl ()
  {
    _wxeInfo = new WxeTemplateControlInfo (this);
  }

  protected override void OnInit (EventArgs e)
  {
    _wxeInfo.Initialize (Context);
    base.OnInit (e);
  }

  public WxePageStep CurrentStep
  {
    get { return _wxeInfo.CurrentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  public IWxePage WxePage
  {
    get { return (IWxePage) base.Page; }
  }
}

}