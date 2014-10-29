using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocTextValueUserControl : DataEditUserControl
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
      TestOutput.SetCurrentValueNormal ((string) LastNameField_Normal.Value);
      TestOutput.SetCurrentValueNoAutoPostBack ((string) LastNameField_NoAutoPostBack.Value);
    }

    private BocTextValueUserControlTestOutput TestOutput
    {
      get { return (BocTextValueUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}