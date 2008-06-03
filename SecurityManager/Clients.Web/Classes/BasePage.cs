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
using System.Globalization;
using System.Threading;
using Remotion.Globalization;
using Remotion.SecurityManager.Clients.Web.Globalization.UI;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  [WebMultiLingualResources (typeof (GlobalResources))]
  public abstract class BasePage : WxePage, IObjectWithResources 
  {
    // types
    private const string c_globalStyleFileUrl = "Style.css";
    private const string c_globalStyleFileKey = "SecurityManagerGlobalStyle";

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected new BaseTransactedFunction CurrentFunction
    {
      get { return (BaseTransactedFunction) base.CurrentFunction; }
    }

    protected virtual IFocusableControl InitialFocusControl
    {
      get { return null; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      RegisterStyleSheets ();
    }

    protected override void OnPreRender (EventArgs e)
    {
      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      if (!IsPostBack && InitialFocusControl != null)
        SetFocus (InitialFocusControl);

      base.OnPreRender (e);
    }

    private void RegisterStyleSheets ()
    {
      string url = ResourceUrlResolver.GetResourceUrl (this, typeof (ResourceUrlResolver), ResourceType.Html, "style.css");

      HtmlHeadAppender.Current.RegisterStylesheetLink (this.GetType () + "style", url);

      if (!HtmlHeadAppender.Current.IsRegistered (c_globalStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (this, typeof (BasePage), ResourceType.Html, c_globalStyleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (c_globalStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    IResourceManager IObjectWithResources.GetResourceManager ()
    {
      return this.GetResourceManager ();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType ();

      if (MultiLingualResources.ExistsResource (type))
        return MultiLingualResources.GetResourceManager (type, true);
      else
        return null;
    }
  }
}
