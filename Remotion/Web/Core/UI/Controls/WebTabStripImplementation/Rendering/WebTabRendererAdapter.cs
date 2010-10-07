// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  public class WebTabRendererAdapter
  {
    private readonly IWebTabRenderer _webTabRenderer;
    private readonly IWebTab _webTab;

    public WebTabRendererAdapter (IWebTabRenderer webTabRenderer, IWebTab webTab)
    {
      ArgumentUtility.CheckNotNull ("webTabRenderer", webTabRenderer);
      ArgumentUtility.CheckNotNull ("webTab", webTab);

      _webTabRenderer = webTabRenderer;
      _webTab = webTab;
    }

    public IWebTabRenderer WebTabRenderer
    {
      get { return _webTabRenderer; }
    }

    public IWebTab WebTab
    {
      get { return _webTab; }
    }

    public void Render (WebTabStripRenderingContext renderingContext, IWebTab tab, bool isEnabled, bool isLast, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("style", style);

      _webTabRenderer.Render (renderingContext, tab, isEnabled, isLast, style);
    }

  }
}