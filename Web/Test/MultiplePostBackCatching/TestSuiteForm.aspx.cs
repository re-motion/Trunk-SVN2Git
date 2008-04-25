using System;
using Remotion.Web.Test.MultiplePostBackCatching;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public partial class TestSuiteForm : TestBasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestSuiteGenerator.GenerateTestCases (this, TestSuiteTable.Rows, "~/MultiplePostbackCatching/TestForm.aspx", "Standard");
    }
  }
}