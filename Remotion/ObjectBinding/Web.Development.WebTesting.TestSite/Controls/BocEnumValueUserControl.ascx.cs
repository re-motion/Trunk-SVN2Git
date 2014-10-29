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
      TestOutput.SetCurrentValueRadioButtonListMultiColumn (
          MarriageStatusField_RadioButtonListMultiColumn.Value != null ? MarriageStatusField_RadioButtonListMultiColumn.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListFlow (
          MarriageStatusField_RadioButtonListFlow.Value != null ? MarriageStatusField_RadioButtonListFlow.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListOrderedList (
          MarriageStatusField_RadioButtonListOrderedList.Value != null ? MarriageStatusField_RadioButtonListOrderedList.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListUnorderedList (
          MarriageStatusField_RadioButtonListUnorderedList.Value != null ? MarriageStatusField_RadioButtonListUnorderedList.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListLabelLeft (
          MarriageStatusField_RadioButtonListLabelLeft.Value != null ? MarriageStatusField_RadioButtonListLabelLeft.Value.ToString() : "");
    }

    private BocEnumValueUserControlTestOutput TestOutput
    {
      get { return (BocEnumValueUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}