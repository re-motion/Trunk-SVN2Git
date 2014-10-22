using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocEnumValueUserControl : DataEditUserControl
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
      TestOutput.SetCurrentValueDropDownListNormal (
          MarriageStatusField_DropDownListNormal.Value != null ? MarriageStatusField_DropDownListNormal.Value.ToString() : "");
      TestOutput.SetCurrentValueDropDownListNoAutoPostBack (MarriageStatusField_DropDownListNoAutoPostBack.Value.ToString());

      TestOutput.SetCurrentValueListBoxNormal (
          MarriageStatusField_ListBoxNormal.Value != null ? MarriageStatusField_ListBoxNormal.Value.ToString() : "");
      TestOutput.SetCurrentValueListBoxNoAutoPostBack (MarriageStatusField_ListBoxNoAutoPostBack.Value.ToString());

      TestOutput.SetCurrentValueRadioButtonListNormal (
          MarriageStatusField_RadioButtonListNormal.Value != null ? MarriageStatusField_RadioButtonListNormal.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListNoAutoPostBack (MarriageStatusField_RadioButtonListNoAutoPostBack.Value.ToString());
    }

    private BocEnumValueUserControlTestOutput TestOutput
    {
      get { return ((Layout) Page.Master).GetTestOutputControl<BocEnumValueUserControlTestOutput>(); }
    }
  }
}