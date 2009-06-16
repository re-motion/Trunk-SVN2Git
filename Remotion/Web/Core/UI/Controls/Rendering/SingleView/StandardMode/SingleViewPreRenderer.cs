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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.SingleView.StandardMode
{
  public class SingleViewPreRenderer : PreRendererBase<ISingleView>, ISingleViewPreRenderer
  {
    public SingleViewPreRenderer (IHttpContext context, ISingleView control)
        : base(context, control)
    {
    }

    public override void PreRender ()
    {
      string keyStyle = typeof (ISingleView).FullName + "_Style";
      string keyScript = typeof (ISingleView).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (keyStyle))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ISingleView), ResourceType.Html, ResourceTheme.Standard, "SingleView.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ISingleView), ResourceType.Html, ResourceTheme.Standard, "Views.js");
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (keyScript, scriptUrl);
      }

      string script = "function adjustViews(){{"
                      + "Views.SetBodyHeightToWindowHeight({0});"
                      + "Views.Adjust({0}, {1}, 1);"
                      + "}}"
                      + "function adjustViewsWithTimeout() {{"
                      + "setTimeout('adjustViews();', 10);"
                      + "}}"
                      + "window.onresize = adjustViewsWithTimeout;";
      script = string.Format (script, Control.ClientID, Control.ViewClientID);
      Control.Page.ClientScript.RegisterClientScriptBlock (typeof (ISingleView), Control.ClientID + "_AdjustViews", script, true);
    }
  }
}