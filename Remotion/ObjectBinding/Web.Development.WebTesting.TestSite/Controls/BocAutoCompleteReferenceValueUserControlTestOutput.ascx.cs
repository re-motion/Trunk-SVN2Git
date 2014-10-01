using System;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocAutoCompleteReferenceValueUserControlTestOutput : UserControl
  {
    public void SetBOUINormal (string uniqueIdentifier)
    {
      BOUINormalLabel.Text = uniqueIdentifier;
    }

    public void SetBOUINoAutoPostBack (string uniqueIdentifier)
    {
      BOUINoAutoPostBackLabel.Text = uniqueIdentifier;
    }

    public void SetActionPerformed (string action, string parameter, string sender)
    {
      ActionPerformedLabel.Text = action;
      ActionPerformedParameterLabel.Text = parameter;
      ActionPerformedSenderLabel.Text = sender;
    }
  }
}