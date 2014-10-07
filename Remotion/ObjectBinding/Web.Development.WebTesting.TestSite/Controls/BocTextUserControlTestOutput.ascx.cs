using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocTextUserControlTestOutput : UserControl
  {
    public void SetCurrentValueNormal (string value)
    {
      NormalCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueNoAutoPostBack (string value)
    {
      NoAutoPostBackCurrentValueLabel.Text = value;
    }
  }
}