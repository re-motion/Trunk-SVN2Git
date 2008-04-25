using System;
using Remotion.Web.UI;

namespace OBWTest.Design
{

public class DesignTestWxeBasePage: TestWxeBasePage
{
  protected override bool IsAbortEnabled
  {
    get { return false; }
  }


  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    HtmlHeadAppender.Current.RegisterStylesheetLink ("design", "Html/Design.css");
  }

}

}
