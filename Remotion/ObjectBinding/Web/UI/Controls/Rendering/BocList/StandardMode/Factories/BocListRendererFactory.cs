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
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories
{
  /// <summary>
  /// Responsible for creating the quirks mode renderers for <see cref="IBocList"/> and its parts except columns - for that,
  /// see <see cref="BocColumnRendererFactory"/>.
  /// </summary>
  public class BocListRendererFactory
      :
          IBocListRendererFactory,
          IBocListMenuBlockRendererFactory,
          IBocListNavigationBlockRendererFactory,
          IBocRowRendererFactory,
          IBocListTableBlockRendererFactory
  {
    public IBocListCientScriptBahavior CreateClientScriptBehavior (HttpContextBase context, IBocList list)
    {
      return new BocListClientScriptBehavior ();
    }

    public IRenderer CreateRenderer (HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new BocListRenderer (context, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocRowRenderer IBocRowRendererFactory.CreateRenderer (HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new BocRowRenderer (context, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListMenuBlockRenderer IBocListMenuBlockRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new BocListMenuBlockRenderer (context, list, CssClassContainer.Instance);
    }

    IBocListNavigationBlockRenderer IBocListNavigationBlockRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new BocListNavigationBlockRenderer (context, list, CssClassContainer.Instance);
    }

    IBocListTableBlockRenderer IBocListTableBlockRendererFactory.CreateRenderer (
        HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new BocListTableBlockRenderer (context, list, CssClassContainer.Instance, serviceLocator);
    }
  }
}