// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Web;

namespace Remotion.Web.UI.Controls.Rendering.WebButton.StandardMode
{
  public class WebButtonPreRenderer : PreRendererBase<IWebButton>, IWebButtonPreRenderer
  {
    public WebButtonPreRenderer (HttpContextBase context, IWebButton control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string scriptKey = typeof (IWebButton).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (scriptKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IWebButton), ResourceType.Html, "WebButton.js");
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (scriptKey, url);
      }

      string styleKey = typeof (IWebButton).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (styleKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IWebButton), ResourceType.Html, ResourceTheme, "WebButton.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (styleKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void PreRender ()
    {
    }
  }
}