using System;
using Remotion.Web.UI;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public partial class TestForm : TestBasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestExpectationsGenerator.GenerateExpectations (this, TestTable.Rows, "~/MultiplePostbackCatching/SutForm.aspx");
      HtmlHeadAppender.Current.SetTitle (TestExpectationsGenerator.GetTestCaseUrlParameter (this) ?? "All Multiple Postback Catcher Tests");
    }
  }
}