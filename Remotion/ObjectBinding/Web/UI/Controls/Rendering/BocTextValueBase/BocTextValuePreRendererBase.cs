// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase
{
  public abstract class BocTextValuePreRendererBase : PreRendererBase<IBocTextValue>, IBocTextValuePreRenderer
  {
    protected BocTextValuePreRendererBase (IHttpContext context, IBocTextValue control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      Control.TextBoxStyle.RegisterJavaScriptInclude (Control, Context, htmlHeadAppender, ResourceTheme);
    }

    protected abstract ResourceTheme ResourceTheme { get; }

    public override void PreRender ()
    {
    }
  }
}