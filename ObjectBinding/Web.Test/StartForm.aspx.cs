using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
  public class StartForm : Page
  {
    protected Button Button1;
    protected WebButton Button1Button;
    protected WebButton Submit1Button;
    protected WebButton Button2Button;
    protected TextBox TextBox1;
    protected BocTextValue BocTextValue1;
    protected HtmlHeadContents HtmlHeadContents;

    #region Web Form Designer generated code

    protected override void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit (e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.Load += new System.EventHandler (this.Page_Load);
    }

    #endregion

    private void Page_Load (object sender, EventArgs e)
    {
    }
  }
}