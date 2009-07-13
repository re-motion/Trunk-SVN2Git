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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode
{
  public class BocAutoCompleteReferenceValuePreRenderer : PreRendererBase<IBocAutoCompleteReferenceValue>
  {
    public BocAutoCompleteReferenceValuePreRenderer (IHttpContext context, IBocAutoCompleteReferenceValue control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string scriptKey = typeof (IBocAutoCompleteReferenceValue).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (IBocAutoCompleteReferenceValue),
              ResourceType.Html,
              ResourceTheme.Standard,
              "BocAutoCompleteReferenceValue.js"));

      string styleKey = typeof (IBocAutoCompleteReferenceValue).FullName + "_Style";
      htmlHeadAppender.RegisterStylesheetLink (
          styleKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (IBocAutoCompleteReferenceValue),
              ResourceType.Html,
              ResourceTheme.Standard,
              "BocAutoCompleteReferenceValue.css"));
    }

    public override void PreRender ()
    {
      
    }
  }
}