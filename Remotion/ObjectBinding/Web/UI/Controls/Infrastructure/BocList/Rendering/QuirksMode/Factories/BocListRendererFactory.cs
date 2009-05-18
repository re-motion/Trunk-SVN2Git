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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories
{
  public class BocListRendererFactory : 
    IBocListRendererFactory,
    IBocListMenuBlockRendererFactory, 
    IBocListNavigationBlockRendererFactory, 
    IBocRowRendererFactory, IBocListTableBlockRendererFactory
  {
    IBocRowRenderer IBocRowRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocRowRenderer (context, writer, list, serviceLocator);
    }

    IBocListMenuBlockRenderer IBocListMenuBlockRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      
      return new BocListMenuBlockRenderer (context, writer, list);
    }

    IBocListNavigationBlockRenderer IBocListNavigationBlockRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocListNavigationBlockRenderer (context, writer, list);
    }

    public IBocListRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocListRenderer (context, writer, list, serviceLocator);
    }

    IBocListTableBlockRenderer IBocListTableBlockRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocListTableBlockRenderer (context, writer, list, serviceLocator);
    }
  }
}