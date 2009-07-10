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
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue
{
  public abstract class BocReferenceValuePreRendererBase : PreRendererBase<IBocReferenceValue>, IBocReferenceValuePreRenderer
  {
    private const string c_scriptFileUrl = "BocReferenceValue.js";
    private const string c_styleFileUrl = "BocReferenceValue.css";

    private static readonly string s_scriptFileKey = typeof (IBocReferenceValue).FullName + "_Script";
    private static readonly string s_startUpScriptKey = typeof (IBocReferenceValue).FullName + "_Startup";
    private static readonly string s_styleFileKey = typeof (IBocReferenceValue).FullName + "_Style";

    protected BocReferenceValuePreRendererBase (IHttpContext context, IBocReferenceValue control)
        : base (context, control)
    {
    }

    public void RegisterHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      if (!htmlHeadAppender.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IBocReferenceValue), ResourceType.Html, ResourceTheme, c_scriptFileUrl);
        htmlHeadAppender.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_styleFileKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IBocReferenceValue), ResourceType.Html, ResourceTheme, c_styleFileUrl);

        htmlHeadAppender.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void PreRender ()
    {
      if (!Control.IsDesignMode && !Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocReferenceValuePreRendererBase), s_startUpScriptKey))
      {
        string script = "BocReferenceValue_InitializeGlobals ('" + Control.NullIdentifier + "');";
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (BocReferenceValuePreRendererBase), s_startUpScriptKey, script);
      }
    }

    protected abstract ResourceTheme ResourceTheme { get; }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      
    }
  }
}