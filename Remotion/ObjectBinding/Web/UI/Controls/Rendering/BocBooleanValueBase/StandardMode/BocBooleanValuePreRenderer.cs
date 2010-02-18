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
using Remotion.Web;
using System.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase.StandardMode
{
  public class BocBooleanValuePreRenderer : BocPreRendererBase<IBocBooleanValue>, IBocBooleanValuePreRenderer
  {
    private static readonly string s_scriptFileKey = typeof (IBocBooleanValue).FullName + "_Script";
    private static readonly string s_styleFileKey = typeof (IBocBooleanValue).FullName + "_Style";

    public BocBooleanValuePreRenderer (HttpContextBase context, IBocBooleanValue control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      if (!htmlHeadAppender.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocBooleanValue), ResourceType.Html, "BocBooleanValue.js");
        htmlHeadAppender.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocBooleanValue), ResourceType.Html, ResourceTheme, "BocBooleanValue.css");
        htmlHeadAppender.RegisterStylesheetLink (s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void PreRender ()
    {
    }
  }
}
