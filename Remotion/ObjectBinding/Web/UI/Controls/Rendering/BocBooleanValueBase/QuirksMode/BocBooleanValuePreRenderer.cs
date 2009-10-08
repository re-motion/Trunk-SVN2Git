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
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase.QuirksMode
{
  public class BocBooleanValuePreRenderer : BocPreRendererBase<IBocBooleanValue>, IBocBooleanValuePreRenderer
  {
    private static readonly string s_scriptFileKey = typeof (IBocBooleanValue).FullName + "_Script";
    private static readonly string s_styleFileKey = typeof (IBocBooleanValue).FullName + "_Style";

    public BocBooleanValuePreRenderer (IHttpContext context, IBocBooleanValue control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (htmlHeadAppender);

      if (!htmlHeadAppender.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocBooleanValue), ResourceType.Html, ResourceTheme.Legacy, "BocBooleanValue.js");
        htmlHeadAppender.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocBooleanValue), ResourceType.Html, ResourceTheme.Legacy, "BocBooleanValue.css");
        htmlHeadAppender.RegisterStylesheetLink (s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void PreRender ()
    {
    }
  }
}