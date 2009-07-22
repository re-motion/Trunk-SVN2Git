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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.StandardMode
{
  public class BocMultilineTextValuePreRenderer : PreRendererBase<IBocMultilineTextValue>, IBocMultilineTextValuePreRenderer
  {
    public BocMultilineTextValuePreRenderer (IHttpContext context, IBocMultilineTextValue control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (IBocMultilineTextValue).FullName + "_Style";
      string styleUrl = ResourceUrlResolver.GetResourceUrl (Control, typeof (IBocMultilineTextValue), ResourceType.Html, "BocMultilineTextValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void PreRender ()
    {
    }
  }
}