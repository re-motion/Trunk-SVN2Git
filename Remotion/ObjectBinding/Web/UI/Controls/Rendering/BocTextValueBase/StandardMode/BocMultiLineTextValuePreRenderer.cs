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
using Remotion.Utilities;
using Remotion.Web;
using System.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.StandardMode
{
  public class BocMultilineTextValuePreRenderer : BocPreRendererBase<IBocMultilineTextValue>, IBocMultilineTextValuePreRenderer
  {
    public BocMultilineTextValuePreRenderer (HttpContextBase context, IBocMultilineTextValue control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (htmlHeadAppender);

      Control.TextBoxStyle.RegisterJavaScriptInclude (Control, Context, htmlHeadAppender, false);

      string styleKey = typeof (IBocMultilineTextValue).FullName + "_Style";
      string styleUrl = ResourceUrlResolver.GetResourceUrl (
          Control, typeof (IBocMultilineTextValue), ResourceType.Html, ResourceTheme, "BocMultilineTextValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void PreRender ()
    {
    }
  }
}
