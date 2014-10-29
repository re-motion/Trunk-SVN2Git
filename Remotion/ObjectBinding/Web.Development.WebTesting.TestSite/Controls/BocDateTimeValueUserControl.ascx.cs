using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocDateTimeValueUserControl : DataEditUserControl
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
      TestOutput.SetCurrentValueNormal (DateOfBirthField_Normal.Value != null ? DateOfBirthField_Normal.Value.Value.ToString() : "invalid");
      TestOutput.SetCurrentValueNoAutoPostBack (
          DateOfBirthField_NoAutoPostBack.Value != null ? DateOfBirthField_NoAutoPostBack.Value.Value.ToString() : "invalid");
      TestOutput.SetCurrentValueDateOnly (DateOfBirthField_DateOnly.Value != null ? DateOfBirthField_DateOnly.Value.Value.ToString() : "invalid");
      TestOutput.SetCurrentValueWithSeconds (
          DateOfBirthField_WithSeconds.Value != null ? DateOfBirthField_WithSeconds.Value.Value.ToString() : "invalid");
    }

    private BocDateTimeValueUserControlTestOutput TestOutput
    {
      get { return ((Layout) Page.Master).GetTestOutputControl<BocDateTimeValueUserControlTestOutput>(); }
    }
  }
}