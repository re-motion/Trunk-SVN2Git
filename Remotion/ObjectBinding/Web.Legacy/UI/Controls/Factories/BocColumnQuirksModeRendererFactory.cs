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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories
{
  /// <summary>
  /// Responsible for creating the quirks mode column renderers.
  /// </summary>
  public class BocColumnQuirksModeRendererFactory
      :
          IBocColumnRendererFactory<BocSimpleColumnDefinition>,
          IBocColumnRendererFactory<BocCompoundColumnDefinition>,
          IBocColumnRendererFactory<BocCommandColumnDefinition>,
          IBocColumnRendererFactory<BocCustomColumnDefinition>,
          IBocColumnRendererFactory<BocDropDownMenuColumnDefinition>,
          IBocColumnRendererFactory<BocRowEditModeColumnDefinition>,
          IBocIndexColumnRendererFactory,
          IBocSelectorColumnRendererFactory
  {
    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocSimpleColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      return new BocSimpleColumnQuirksModeRenderer (context, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCompoundColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      return new BocCompoundColumnQuirksModeRenderer (context, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCommandColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      return new BocCommandColumnQuirksModeRenderer (context, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCustomColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      return new BocCustomColumnQuirksModeRenderer (context, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocDropDownMenuColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      return new BocDropDownMenuColumnQuirksModeRenderer (context, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocRowEditModeColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      return new BocRowEditModeColumnQuirksModeRenderer (context, list, columnDefinition, CssClassContainer.Instance);
    }

    IBocIndexColumnRenderer IBocIndexColumnRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new BocIndexColumnQuirksModeRenderer (context, list, CssClassContainer.Instance);
    }

    IBocSelectorColumnRenderer IBocSelectorColumnRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new BocSelectorColumnQuirksModeRenderer (context, list, CssClassContainer.Instance);
    }
  }
}