using System;
using Remotion.Globalization;
using Remotion.Web.UI;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
public class SingleBocTestWxeBasePage: TestWxeBasePage
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    this.EnableAbort = true;
    this.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
  }

}

}
