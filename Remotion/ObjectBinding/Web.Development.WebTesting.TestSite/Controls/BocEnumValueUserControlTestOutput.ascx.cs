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

    public void SetCurrentValueRadioButtonListMultiColumn (string value)
    {
      RadioButtonListMultiColumnCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueRadioButtonListFlow (string value)
    {
      RadioButtonListFlowCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueRadioButtonListOrderedList (string value)
    {
      RadioButtonListOrderedListCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueRadioButtonListUnorderedList (string value)
    {
      RadioButtonListUnorderedListCurrentValueLabel.Text = value;
    }

    public void SetCurrentValueRadioButtonListLabelLeft (string value)
    {
      RadioButtonListLabelLeftCurrentValueLabel.Text = value;
    }
  }
}