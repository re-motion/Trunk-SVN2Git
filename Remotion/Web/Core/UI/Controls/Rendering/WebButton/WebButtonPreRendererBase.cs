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

namespace Remotion.Web.UI.Controls.Rendering.WebButton
{
  public abstract class WebButtonPreRendererBase : PreRendererBase<IWebButton>, IWebButtonPreRenderer
  {
    protected WebButtonPreRendererBase (IHttpContext context, IWebButton control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string scriptKey = typeof (IWebButton).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (scriptKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IWebButton), ResourceType.Html, ResourceTheme, "WebButton.js");
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (scriptKey, url);
      }

      string styleKey = typeof (IWebButton).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (styleKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IWebButton), ResourceType.Html, ResourceTheme, "WebButton.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (styleKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    protected abstract ResourceTheme ResourceTheme { get; }

    public override void PreRender ()
    {

    }
  }
}