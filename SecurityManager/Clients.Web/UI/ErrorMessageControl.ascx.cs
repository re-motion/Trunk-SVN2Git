using System;
using System.Web.UI;
using Remotion.SecurityManager.Clients.Web.Globalization.UI;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public partial class ErrorMessageControl : UserControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      ErrorsOnPageLabel.Text = GlobalResources.ErrorMessage;
    }

    public void ShowError ()
    {
      ErrorsOnPageLabel.Visible = true;
    }
  }
}