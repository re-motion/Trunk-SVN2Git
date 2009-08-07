// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase
{
  public abstract class BocBooleanValuePreRendererBase : PreRendererBase<IBocBooleanValue>
  {
    private const string c_scriptFileUrl = "BocBooleanValue.js";
    private const string c_styleFileUrl = "BocBooleanValue.css";

    private static readonly string s_scriptFileKey = typeof (BocBooleanValue).FullName + "_Script";
    private static readonly string s_styleFileKey = typeof (BocBooleanValue).FullName + "_Style";

    protected BocBooleanValuePreRendererBase (IHttpContext context, IBocBooleanValue control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      if (!htmlHeadAppender.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocBooleanValue), ResourceType.Html, ResourceTheme, c_scriptFileUrl);
        htmlHeadAppender.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      if (!htmlHeadAppender.IsRegistered (s_styleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (BocBooleanValue), ResourceType.Html, ResourceTheme, c_styleFileUrl);
        htmlHeadAppender.RegisterStylesheetLink (s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void PreRender ()
    {
      
    }

    protected abstract ResourceTheme ResourceTheme { get; }
  }
}