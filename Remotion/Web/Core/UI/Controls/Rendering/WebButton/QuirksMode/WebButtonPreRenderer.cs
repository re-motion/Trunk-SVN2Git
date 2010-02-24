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
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.WebButton.QuirksMode
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for quirks mode rendering of <see cref="IWebButton"/> controls.
  /// </summary>
  public class WebButtonPreRenderer : RendererBase<IWebButton>
  {
    public WebButtonPreRenderer (HttpContextBase context, IWebButton control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptKey = typeof (IWebButton).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IWebButton), ResourceType.Html, ResourceTheme.Legacy, "WebButton.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptKey, url);
      }

      string styleKey = typeof (IWebButton).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IWebButton), ResourceType.Html, ResourceTheme.Legacy, "WebButton.css");
        htmlHeadAppender.RegisterStylesheetLink (styleKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void Render (HtmlTextWriter writer)
    {
      throw new NotSupportedException ("The WebButton does not support customized rendering.");
    }
  }
}
