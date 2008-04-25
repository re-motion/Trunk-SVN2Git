using System;
using Remotion.Web.UI;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public partial class UpdatePanelTestForm : TestBasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestExpectationsGenerator.GenerateExpectations (this, TestTable.Rows, "~/MultiplePostbackCatching/UpdatePanelSutForm.aspx");
      HtmlHeadAppender.Current.SetTitle (TestExpectationsGenerator.GetTestCaseUrlParameter (this) ?? "All Multiple Postback Catcher Tests");
    }
  }
}