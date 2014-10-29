using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocDateTimeValueUserControlTestOutput : UserControl
  {
    public void SetCurrentValueNormal (string value)
    {
      NormalCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueNoAutoPostBack (string value)
    {
      NoAutoPostBackCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueDateOnly (string value)
    {
      DateOnlyCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueWithSeconds (string value)
    {
      WithSecondsCurrentValueLabel.Text = value;
    }
  }
}