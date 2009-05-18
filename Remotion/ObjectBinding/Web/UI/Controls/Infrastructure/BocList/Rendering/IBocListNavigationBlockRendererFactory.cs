// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering
{
  public interface IBocListNavigationBlockRendererFactory
  {
    IBocListNavigationBlockRenderer CreateRenderer (HtmlTextWriter writer, IBocList list);
  }
}