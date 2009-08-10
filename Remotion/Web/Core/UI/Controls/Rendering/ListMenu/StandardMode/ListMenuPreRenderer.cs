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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.ListMenu.StandardMode
{
  /// <summary>
  /// Responsible for registering scripts and the style sheet for <see cref="ListMenu"/> controls in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuPreRenderer : ListMenuPreRendererBase
  {
    public ListMenuPreRenderer (IHttpContext context, IListMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterJQueryJavaScriptInclude (Control);

      string scriptFileKey = typeof (IListMenu).FullName + "_Script";
      string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (Control, GetType (), ResourceType.Html, "ListMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptFileUrl);

      string styleSheetKey = typeof (IListMenu).FullName + "_Style";
      string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (Control, GetType (), ResourceType.Html, ResourceTheme, "ListMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }
  }
}