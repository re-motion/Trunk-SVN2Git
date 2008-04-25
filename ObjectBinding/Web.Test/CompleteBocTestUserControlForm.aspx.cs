using System;
using Remotion.Web.UI.Controls;

namespace OBWTest
{

  public class CompleteBocUserControlForm : SingleBocTestWxeBasePage

{
    protected ValidationStateViewer ValidationStateViewer1;
  protected HtmlHeadContents HtmlHeadContents;

	private void Page_Load(object sender, EventArgs e)
	{
		// Put user code to initialize the page here
	}

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

	}

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
