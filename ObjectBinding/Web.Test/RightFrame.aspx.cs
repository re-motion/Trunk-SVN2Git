using System;
using System.Collections;
using System.Web.UI;

namespace OBWTest
{
	/// <summary>
	/// Summary description for LeftFrame.
	/// </summary>
	public class RightFrame : Page
	{
		private void Page_Load(object sender, EventArgs e)
		{
      ArrayList global = (ArrayList) Session["Global"];
      int hashcode = global.GetHashCode();
      lock (global.SyncRoot)
      {
        global.Add (Guid.NewGuid());
      }
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
