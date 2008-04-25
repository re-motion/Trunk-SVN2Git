using System;
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

public class EmptyUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}
	
	/// <summary>
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{

  }
  #endregion
}

}
