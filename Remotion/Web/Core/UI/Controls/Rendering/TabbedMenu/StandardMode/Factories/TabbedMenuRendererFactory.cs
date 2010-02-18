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
using System.Web;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMenu.StandardMode.Factories
{
  /// <summary>
  /// Responsible for creating quirks mode renderers for <see cref="TabbedMenuRenderer"/> controls and <see cref="MenuTab"/> items.
  /// </summary>
  public class TabbedMenuRendererFactory : ITabbedMenuRendererFactory, IMenuTabRendererFactory
  {
    public IWebTabRenderer CreateRenderer (HttpContextBase context, HtmlTextWriter writer, IWebTabStrip control, IMenuTab tab)
    {
      return new MenuTabRenderer (context, writer, control, tab);
    }

    public ITabbedMenuRenderer CreateRenderer (HttpContextBase context, HtmlTextWriter writer, ITabbedMenu control)
    {
      return new TabbedMenuRenderer (context, writer, control);
    }

    public ITabbedMenuPreRenderer CreatePreRenderer (HttpContextBase context, ITabbedMenu menu)
    {
      return new TabbedMenuPreRenderer (context, menu);
    }
  }
}
