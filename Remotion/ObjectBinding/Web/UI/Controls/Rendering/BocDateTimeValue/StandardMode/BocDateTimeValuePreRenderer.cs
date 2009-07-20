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
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.StandardMode
{
  public class BocDateTimeValuePreRenderer : PreRendererBase<IBocDateTimeValue>, IBocDateTimeValuePreRenderer
  {
    public BocDateTimeValuePreRenderer (IHttpContext context, IBocDateTimeValue control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string scriptKey = typeof (IBocDateTimeValue).FullName + "_Script";
      string scriptFile = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocDateTimeValue), ResourceType.Html, ResourceTheme.Standard, "BocDateTimeValue.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptKey, scriptFile);

      string styleKey = typeof (IBocDateTimeValue).FullName + "_Style";
      string styleFile = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocDateTimeValue), ResourceType.Html, ResourceTheme.Standard, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    public override void PreRender ()
    {
      string key = Control.UniqueID + "_AdjustPositions";
      string script = string.Format(@"$(document).ready( function(){{ 
  $(window).bind('resize', function(e){{ 
      BocDateTimeValue.AdjustPositions($('#{0}')); 
  }});
  " + "setTimeout(\"BocDateTimeValue.AdjustPositions($('#{0}'));\", 10);" + @"
}});"
      , Control.ClientID);
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (IBocDateTimeValue), key, script);
    }
  }
}