using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocBooleanValueUserControlTestOutput : UserControl
  {
    public void SetCurrentValueNormal (string value)
    {
      NormalCurrentValueLabel.Text = value;
    }
    
    public void SetCurrentValueTriState (string value)
    {
      TriStateCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueNoAutoPostBack (string value)
    {
      NoAutoPostBackCurrentValueLabel.Text = value;
    }
  }
}