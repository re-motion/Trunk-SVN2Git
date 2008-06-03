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
using System.Web.UI;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public partial class SecurityManagerMasterPage : MasterPage
  {
    private const string c_contentViewStyleFileUrl = "ContentViewStyle.css";
    private const string c_contentViewStyleFileKey = "SecurityManagerContentViewStyle";

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!HtmlHeadAppender.Current.IsRegistered (c_contentViewStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            this, typeof (SecurityManagerMasterPage), ResourceType.Html, c_contentViewStyleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (c_contentViewStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}
