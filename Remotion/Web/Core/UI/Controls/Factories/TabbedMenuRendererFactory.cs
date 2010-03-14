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
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;

namespace Remotion.Web.UI.Controls.Factories
{
  /// <summary>
  /// Responsible for creating standard mode renderers for <see cref="MenuTab"/> controls and <see cref="TabbedMenuRenderer"/> items.
  /// </summary>
  public class TabbedMenuRendererFactory : ITabbedMenuRendererFactory, IMenuTabRendererFactory
  {
    public IWebTabRenderer CreateRenderer (HttpContextBase context, IWebTabStrip control, IMenuTab tab)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("tab", tab);

      return new MenuTabRenderer (context, control, tab);
    }

    public IRenderer CreateRenderer (HttpContextBase context, ITabbedMenu control, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new TabbedMenuRenderer (context, control, serviceLocator.GetInstance<IResourceUrlFactory> ());
    }
  }
}