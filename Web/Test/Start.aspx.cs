using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml.Serialization;
using System.IO;
using System.Text;
namespace Remotion.Web.Test
{
	/// <summary>
	/// Summary description for Start.
	/// </summary>
	public class Start : System.Web.UI.Page
	{
    protected System.Web.UI.WebControls.Button ResetSessionButton;
    protected System.Web.UI.WebControls.HyperLink HyperLink1;
  
		private void Page_Load(object sender, System.EventArgs e)
		{
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
      this.ResetSessionButton.Click += new System.EventHandler(this.ResetSessionButton_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void ResetSessionButton_Click(object sender, System.EventArgs e)
    {
			Session.Clear();
    }
	}
}
