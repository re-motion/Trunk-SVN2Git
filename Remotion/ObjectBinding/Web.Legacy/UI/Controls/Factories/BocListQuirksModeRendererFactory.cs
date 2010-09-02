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
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls;
using CssClassContainer = Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering.CssClassContainer;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories
{
  /// <summary>
  /// Responsible for creating the quirks mode renderer for <see cref="IBocList"/> and its parts except columns - for that,
  /// see <see cref="BocColumnQuirksModeRendererFactory"/>.
  /// </summary>
  public class BocListQuirksModeRendererFactory : IBocListRendererFactory
  {
    public IRenderer CreateRenderer (HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new BocListQuirksModeRenderer (
          context,
          list,
          CssClassContainer.Instance,
          new BocListTableBlockQuirksModeRenderer (
              context,
              list,
              CssClassContainer.Instance,
              new BocRowQuirksModeRenderer (context, list, CssClassContainer.Instance, serviceLocator)),
          new BocListNavigationBlockQuirksModeRenderer (context, list, CssClassContainer.Instance),
          new BocListMenuBlockQuirksModeRenderer (context, list, CssClassContainer.Instance)
          );
    }
  }
}