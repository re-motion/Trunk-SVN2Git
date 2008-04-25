using System;
using System.Collections;
using System.Web.UI;

namespace OBWTest
{
	/// <summary>
	/// Summary description for Frameset.
	/// </summary>
	public class Frameset : Page
	{
		private void Page_Load(object sender, EventArgs e)
		{
      //  Would normally be in SessionStart
      if (! IsPostBack)
        Session["Global"] = new ArrayList();
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
