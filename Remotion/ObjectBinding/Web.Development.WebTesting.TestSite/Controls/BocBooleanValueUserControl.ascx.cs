using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocBooleanValueUserControl : DataEditUserControl
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
      TestOutput.SetCurrentValueNormal (DeceasedField_Normal.Value.ToString());
      TestOutput.SetCurrentValueNoAutoPostBack (DeceasedField_NoAutoPostBack.Value.ToString());
      TestOutput.SetCurrentValueTriState (DeceasedField_TriState.Value.ToString());
    }

    private BocBooleanValueUserControlTestOutput TestOutput
    {
      get { return ((Layout) Page.Master).GetTestOutputControl<BocBooleanValueUserControlTestOutput>(); }
    }
  }
}