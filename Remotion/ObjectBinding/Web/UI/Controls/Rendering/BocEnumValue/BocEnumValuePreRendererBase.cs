// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocEnumValue
{
  public abstract class BocEnumValuePreRendererBase : PreRendererBase<IBocEnumValue>, IBocEnumValuePreRenderer
  {
    private const string c_styleFileName = "BocEnumValue.css";
    private static readonly string s_styleKey = typeof (IBocEnumValue).FullName + "_Style";

    protected BocEnumValuePreRendererBase (IHttpContext context, IBocEnumValue control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IBocEnumValue), ResourceType.Html, ResourceTheme, c_styleFileName);
      htmlHeadAppender.RegisterStylesheetLink (s_styleKey, url, HtmlHeadAppender.Priority.Library);
    }

    protected abstract ResourceTheme ResourceTheme { get; }

    public override void PreRender ()
    {
    }
  }
}