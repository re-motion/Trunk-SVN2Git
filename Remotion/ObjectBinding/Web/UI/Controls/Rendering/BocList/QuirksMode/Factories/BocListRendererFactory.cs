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
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories
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
    IBocRowRenderer IBocRowRendererFactory.CreateRenderer (HttpContextBase context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new StandardMode.BocRowRenderer (context, writer, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListPreRenderer IBocListRendererFactory.CreatePreRenderer (HttpContextBase context, IBocList list)
    {
      return new BocListPreRenderer (context, list, CssClassContainer.Instance);
    }

    IBocListMenuBlockRenderer IBocListMenuBlockRendererFactory.CreateRenderer (HttpContextBase context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new StandardMode.BocListMenuBlockRenderer (context, writer, list, CssClassContainer.Instance);
    }

    IBocListNavigationBlockRenderer IBocListNavigationBlockRendererFactory.CreateRenderer (HttpContextBase context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new StandardMode.BocListNavigationBlockRenderer (context, writer, list, CssClassContainer.Instance);
    }

    public IBocListRenderer CreateRenderer (HttpContextBase context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new StandardMode.BocListRenderer (context, writer, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListTableBlockRenderer IBocListTableBlockRendererFactory.CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new StandardMode.BocListTableBlockRenderer (context, writer, list, CssClassContainer.Instance, serviceLocator);
    }
  }
}
