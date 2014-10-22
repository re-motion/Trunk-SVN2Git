using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocEnumValueUserControlTestOutput : UserControl
  {
    public void SetCurrentValueDropDownListNormal (string value)
    {
      DropDownListNormalCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueDropDownListNoAutoPostBack (string value)
    {
      DropDownListNoAutoPostBackCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueListBoxNormal (string value)
    {
      ListBoxNormalCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueListBoxNoAutoPostBack (string value)
    {
      ListBoxNoAutoPostBackCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueRadioButtonListNormal (string value)
    {
      RadioButtonListNormalCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueRadioButtonListNoAutoPostBack (string value)
    {
      RadioButtonListNoAutoPostBackCurrentValueLabel.Text = value;
    }
  }
}