// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue
{
  /// <summary>
  /// Interface for factories creating <see cref="IBocDateTimeValueRenderer"/> renderers.
  /// </summary>
  public interface IBocDateTimeValueRendererFactory
  {
    IBocDateTimeValueRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocDateTimeValue control);
  }
}