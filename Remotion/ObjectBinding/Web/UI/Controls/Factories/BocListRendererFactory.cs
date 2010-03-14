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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Factories
{
  /// <summary>
  /// Responsible for creating the standard mode renderer for <see cref="IBocList"/> and its parts except columns - for that,
  /// see <see cref="BocColumnRendererFactory"/>.
  /// </summary>
  public class BocListRendererFactory : IBocListRendererFactory
  {
    public IRenderer CreateRenderer (HttpContextBase context, IBocList control, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocListRenderer (
          context,
          control,
          serviceLocator.GetInstance<IResourceUrlFactory>(),
          CssClassContainer.Instance,
          new BocListTableBlockRenderer (
              context,
              control,
              CssClassContainer.Instance,
              new BocRowRenderer (context, control, CssClassContainer.Instance, serviceLocator)),
          new BocListNavigationBlockRenderer (context, control, CssClassContainer.Instance),
          new BocListMenuBlockRenderer (context, control, CssClassContainer.Instance)
          );
    }
  }
}