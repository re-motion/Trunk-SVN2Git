/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;

namespace Remotion.Web.Test
{
  /// <summary>
  /// Summary description for Global.
  /// </summary>
  public class Global : System.Web.HttpApplication
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public Global()
    {
      InitializeComponent();
    }	
		
    protected void Application_Start(Object sender, EventArgs e)
    {
      log4net.Config.XmlConfigurator.Configure();
    }
 
    protected void Session_Start(Object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {

    }

    protected void Application_EndRequest(Object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {

    }

    protected void Application_Error(Object sender, EventArgs e)
    {
      //      string appPath = Request.ApplicationPath;
      //
      //      if (!appPath.EndsWith ("/"))
      //        appPath += "/";
      //
      //      Server.Transfer (appPath + "Start.aspx");
    }

    protected void Session_End(Object sender, EventArgs e)
    {

    }

    protected void Application_End(Object sender, EventArgs e)
    {

    }
			
    #region Web Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {    
      this.components = new System.ComponentModel.Container();
    }
    #endregion
  }
}
