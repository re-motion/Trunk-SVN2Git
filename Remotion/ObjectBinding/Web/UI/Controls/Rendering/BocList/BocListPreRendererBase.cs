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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList
{
  public abstract class BocListPreRendererBase : PreRendererBase<IBocList>, IBocListPreRenderer
  {
    private const string c_scriptFileUrl = "BocList.js";
    private const string c_styleFileUrl = "BocList.css";

    private static readonly string s_scriptFileKey = typeof (IBocList).FullName + "_Script";
    private static readonly string s_styleFileKey = typeof (IBocList).FullName + "_Style";
    private static readonly string s_startUpScriptKey = typeof (IBocList).FullName + "_Startup";

    private readonly CssClassContainer _cssClasses;

    protected BocListPreRendererBase (IHttpContext context, IBocList control, CssClassContainer cssClassContainer)
      : base (context, control)
    {
      _cssClasses = cssClassContainer;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterJQueryJavaScriptInclude (Control);

      if (!htmlHeadAppender.IsRegistered (s_styleFileKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IBocList), ResourceType.Html, ResourceTheme, c_styleFileUrl);
        htmlHeadAppender.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
      }

      if (!htmlHeadAppender.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IBocList), ResourceType.Html, ResourceTheme, c_scriptFileUrl);
        htmlHeadAppender.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      Control.EditModeControlFactory.RegisterHtmlHeadContents (Context, htmlHeadAppender);
    }

    protected abstract ResourceTheme ResourceTheme { get; }

    public override void PreRender ()
    {
      if (!Control.HasClientScript)
        return;

      // Startup script initalizing the global values of the script.
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocListPreRendererBase), s_startUpScriptKey))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}');",
            CssClasses.DataRow,
            CssClasses.DataRowSelected);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof(BocListPreRendererBase), s_startUpScriptKey, script);
      }
    }

    public abstract bool IsBrowserCapableOfScripting{ get; }

    protected CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }
  }
}