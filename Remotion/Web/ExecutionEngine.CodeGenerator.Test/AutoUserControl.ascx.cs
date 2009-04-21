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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Test
{
  // <WxeFunction codeBehindType="Test.AutoUserControl" markupFile="AutoUserControl.ascx" functionBaseType="WxeFunction" mode="UserControl">
  //   <Parameter name="InArg" type="String" required="true"/>
  //   <Parameter name="InOutArg" type="String" direction="InOut" />
  //   <ReturnValue type="String"/>
  //   <Variable name="Suffix" type="String" />
  // </WxeFunction>
  public partial class AutoUserControl : WxeUserControl
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      IsPostBackLabel.Text = IsUserControlPostBack.ToString();
      InArgField.Text = InArg + Suffix;
      string inOutParam = InOutArgField.Text + Suffix;
      InOutArgField.Text = inOutParam;
    }

    protected override void LoadViewState (object savedState)
    {
      base.LoadViewState (savedState);
    }
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.Render (writer);
    }
    protected void Button1_Click (object sender, EventArgs e)
    {
      ReturnValue = "thank you";
      Return ();
    }
  }
}