using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocMultilineTextValueUserControlTestOutput : UserControl
  {
    public void SetCurrentValueNormal (string value)
    {
      NormalCurrentValueLabel.Text = Server.HtmlEncode(value);
    }

    public void SetCurrentValueNoAutoPostBack (string value)
    {
      NoAutoPostBackCurrentValueLabel.Text = Server.HtmlEncode(value);
    }
  }
}