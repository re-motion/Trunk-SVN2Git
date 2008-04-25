using System;
using Remotion.Web.Test.MultiplePostBackCatching;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public partial class UpdatePanelTestSuiteForm : TestBasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestSuiteGenerator.GenerateTestCases (this, TestSuiteTable.Rows, "~/MultiplePostbackCatching/UpdatePanelTestForm.aspx", "UpdatePanel");
    }
  }
}