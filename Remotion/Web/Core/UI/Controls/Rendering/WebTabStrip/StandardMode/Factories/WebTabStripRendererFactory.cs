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
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.StandardMode.Factories
{
  /// <summary>
  /// Responsible for creating standard mode renderers for <see cref="WebTabStrip"/> controls and <see cref="WebTab"/> items.
  /// </summary>
  public class WebTabStripRendererFactory : IWebTabStripRendererFactory, IWebTabRendererFactory
  {
    public IWebTabStripRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control)
    {
      return new WebTabStripRenderer (context, writer, control);
    }

    public IWebTabStripPreRenderer CreatePreRenderer (IHttpContext context, IWebTabStrip control)
    {
      return new WebTabStripPreRenderer (context, control);
    }

    IWebTabRenderer IWebTabRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control, IWebTab tab)
    {
      return new WebTabRenderer (context, writer, control, tab);
    }
  }
}
