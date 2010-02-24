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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.ListMenu.QuirksMode
{
  /// <summary>
  /// Responsible for registering scripts and the style sheet for <see cref="ListMenu"/> controls in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuRenderer : StandardMode.ListMenuRenderer
  {
    public ListMenuRenderer (HttpContextBase context, IListMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      // Do not call base implementation
      //base.RegisterHtmlHeadContents

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control);

      string scriptFileKey = typeof (ListMenuRenderer).FullName + "_Script";
      string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (
          Control, typeof (ListMenuRenderer), ResourceType.Html, ResourceTheme.Legacy, "ListMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptFileUrl);

      string styleSheetKey = typeof (ListMenuRenderer).FullName + "_Style";
      string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          Control, typeof (ListMenuRenderer), ResourceType.Html, ResourceTheme.Legacy, "ListMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }
  }
}