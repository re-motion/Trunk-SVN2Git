// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Obsolete;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class UserControl1 : WxeUserControl
  {
    protected TextBox TextBox1;
    protected Button Stay;
    protected Button Sub;
    protected Label Label1;
    protected Button Next;

    private static int s_counter;

    private void Page_Load (object sender, EventArgs e)
    {
      if (! IsPostBack)
      {
        ++ s_counter;
        ViewState["Counter"] = s_counter.ToString();
      }
      Label1.Text = (string) ViewState["Counter"];
    }

    #region Web Form Designer generated code

    protected override void OnInitComplete (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInitComplete (e);
    }

    /// <summary>
    ///		Required method for Designer support - do not modify
    ///		the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.Stay.Click += new System.EventHandler (this.Stay_Click);
      this.Sub.Click += new System.EventHandler (this.Sub_Click);
      this.Next.Click += new System.EventHandler (this.Next_Click);
      this.Load += new System.EventHandler (this.Page_Load);
    }

    #endregion

    private void Stay_Click (object sender, EventArgs e)
    {
    }

    private void Sub_Click (object sender, EventArgs e)
    {
      ViewState["Counter"] += " Sub_Click";
      WxePage.ExecuteFunctionNoRepost (new WebForm1.SubFunction ("usercontrol var1", "usercontrol var2"), (IControl) sender);
    }

    private void Next_Click (object sender, EventArgs e)
    {
      WxePage.ExecuteNextStep();
    }
  }
}