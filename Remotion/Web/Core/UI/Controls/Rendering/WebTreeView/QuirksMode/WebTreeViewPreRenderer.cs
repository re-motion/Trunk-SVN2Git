// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.WebTreeView.QuirksMode
{
  public class WebTreeViewPreRenderer : WebTreeViewPreRendererBase
  {
    public WebTreeViewPreRenderer (IHttpContext context, IWebTreeView control)
        : base(context, control)
    {
    }

    protected override ResourceTheme ResourceTheme
    {
      get { return ResourceTheme.Legacy; }
    }
  }
}