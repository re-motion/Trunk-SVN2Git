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

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  public class TabbedMultiViewPreRenderer : PreRendererBase<ITabbedMultiView>, ITabbedMultiViewPreRenderer
  {
    public TabbedMultiViewPreRenderer (IHttpContext context, ITabbedMultiView control)
        : base(context, control)
    {
    }

    public override void PreRender ()
    {
      string keyStyle = typeof (ITabbedMultiView).FullName + "_Style";
      string keyScript = typeof (ITabbedMultiView).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (keyStyle))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, ResourceTheme.Standard, "TabbedMultiView.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

        string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, ResourceTheme.Standard, "Views.js");
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (keyScript, scriptFileUrl);
      }

      string script = "function adjustViews(){{"
                      + "Views.SetBodyHeightToWindowHeight({0});"
                      + "Views.AdjustTop({0}, {1}, 1); "
                      + "Views.Adjust({0}, {2}, 1);"
                      + "}}"
                      + "function adjustViewsWithTimeout() {{"
                      + "setTimeout('adjustViews();', 10);"
                      + "}}"
                      + "window.onresize = adjustViewsWithTimeout;";
      script = string.Format (script, Control.ClientID, Control.TabStripContainerClientID, Control.ActiveViewClientID);
      Control.Page.ClientScript.RegisterClientScriptBlock (typeof (ITabbedMultiView), Control.ClientID + "_AdjustViews", script, true);
    }
  }
}