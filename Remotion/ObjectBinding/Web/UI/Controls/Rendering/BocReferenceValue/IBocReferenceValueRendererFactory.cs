// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue
{
  /// <summary>
  /// Interface for factory creating renderers for <see cref="BocReferenceValue"/> controls.
  /// </summary>
  public interface IBocReferenceValueRendererFactory
  {
    IBocReferenceValueRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocReferenceValue control);
  }
}