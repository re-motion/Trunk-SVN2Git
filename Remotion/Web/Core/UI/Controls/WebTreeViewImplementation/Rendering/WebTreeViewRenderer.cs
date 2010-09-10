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

namespace Remotion.Web.UI.Controls.WebTreeViewImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for standard mode rendering of <see cref="WebTreeView"/> controls.
  /// <seealso cref="IWebTreeView"/>
  /// </summary>
  public class WebTreeViewRenderer : RendererBase<IWebTreeView>
  {
    public WebTreeViewRenderer (HttpContextBase context, IWebTreeView control, IResourceUrlFactory resourceUrlFactory)
        : base (context, control, resourceUrlFactory)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (WebTreeViewRenderer).FullName + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (WebTreeViewRenderer), ResourceType.Html, "TreeView.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      Render (new WebTreeViewRenderingContext (Context, writer, Control));
    }

    public void Render (WebTreeViewRenderingContext renderingContext)
    {
      throw new NotSupportedException ("The WebTreeView does not support customized rendering.");
    }
  }
}