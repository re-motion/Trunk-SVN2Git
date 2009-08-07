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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase
{
  public abstract class BocCheckboxPreRendererBase : PreRendererBase<IBocCheckBox>
  {
    private const string c_scriptFileUrl = "BocCheckBox.js";
    private const string c_styleFileUrl = "BocCheckBox.css";

    private static readonly string s_scriptFileKey = typeof (BocCheckBox).FullName + "_Script";
    private static readonly string s_styleFileKey = typeof (BocCheckBox).FullName + "_Style";

    protected BocCheckboxPreRendererBase (IHttpContext context, IBocCheckBox control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      if (!htmlHeadAppender.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocCheckBox), ResourceType.Html, ResourceTheme, c_scriptFileUrl);
        htmlHeadAppender.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocCheckBox), ResourceType.Html, ResourceTheme, c_styleFileUrl);
        htmlHeadAppender.RegisterStylesheetLink (s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    protected abstract ResourceTheme ResourceTheme { get; }

    public override void PreRender ()
    {
    }
  }
}