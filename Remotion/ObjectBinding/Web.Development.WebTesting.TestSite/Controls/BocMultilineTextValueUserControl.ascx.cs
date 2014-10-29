using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocMultilineTextValueUserControl : DataEditUserControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput();
    }

    private void SetTestOutput ()
    {
      TestOutput.SetCurrentValueNormal (string.Join (" NL ", CVField_Normal.Value));
      TestOutput.SetCurrentValueNoAutoPostBack (string.Join (" NL ", CVField_NoAutoPostBack.Value));
    }

    private BocMultilineTextValueUserControlTestOutput TestOutput
    {
      get { return (BocMultilineTextValueUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}