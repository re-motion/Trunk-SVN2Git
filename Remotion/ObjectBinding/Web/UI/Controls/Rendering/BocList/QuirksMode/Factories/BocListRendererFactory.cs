// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

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
    IBocRowRenderer IBocRowRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocRowRenderer (context, writer, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListPreRenderer IBocListRendererFactory.CreatePreRenderer (IHttpContext context, IBocList list)
    {
      return new BocListPreRenderer (context, list, CssClassContainer.Instance);
    }

    IBocListMenuBlockRenderer IBocListMenuBlockRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocListMenuBlockRenderer (context, writer, list, CssClassContainer.Instance);
    }

    IBocListNavigationBlockRenderer IBocListNavigationBlockRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocListNavigationBlockRenderer (context, writer, list, CssClassContainer.Instance);
    }

    public IBocListRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocListRenderer (context, writer, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListTableBlockRenderer IBocListTableBlockRendererFactory.CreateRenderer (
        IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocListTableBlockRenderer (context, writer, list, CssClassContainer.Instance, serviceLocator);
    }
  }
}