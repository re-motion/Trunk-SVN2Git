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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories
{
  /// <summary>
  /// Responsible for creating the quirks mode column renderers.
  /// </summary>
  public class BocColumnQuirksModeRendererFactory
      :
          IBocSimpleColumnRendererFactory,
          IBocCompoundColumnRendererFactory,
          IBocCommandColumnRendererFactory,
          IBocCustomColumnRendererFactory,
          IBocDropDownMenuColumnRendererFactory,
          IBocRowEditModeColumnRendererFactory,
          IBocIndexColumnRendererFactory,
          IBocSelectorColumnRendererFactory
  {
    private readonly BocListQuirksModeCssClassDefinition _bocListQuirksModeCssClassDefinition;

    public BocColumnQuirksModeRendererFactory (BocListQuirksModeCssClassDefinition bocListQuirksModeCssClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("bocListQuirksModeCssClassDefinition", bocListQuirksModeCssClassDefinition);

      _bocListQuirksModeCssClassDefinition = bocListQuirksModeCssClassDefinition;
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocSimpleColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocSimpleColumnQuirksModeRenderer (context, list, columnDefinition, _bocListQuirksModeCssClassDefinition, columnIndex);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCompoundColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocCompoundColumnQuirksModeRenderer (context, list, columnDefinition, _bocListQuirksModeCssClassDefinition, columnIndex);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCommandColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocCommandColumnQuirksModeRenderer (context, list, columnDefinition, _bocListQuirksModeCssClassDefinition, columnIndex);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCustomColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocCustomColumnQuirksModeRenderer (context, list, columnDefinition, _bocListQuirksModeCssClassDefinition, columnIndex);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocDropDownMenuColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocDropDownMenuColumnQuirksModeRenderer (context, list, columnDefinition, _bocListQuirksModeCssClassDefinition, columnIndex);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocRowEditModeColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocRowEditModeColumnQuirksModeRenderer (context, list, columnDefinition, _bocListQuirksModeCssClassDefinition, columnIndex);
    }

    IBocIndexColumnRenderer IBocIndexColumnRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new BocIndexColumnQuirksModeRenderer (context, list, _bocListQuirksModeCssClassDefinition);
    }

    IBocSelectorColumnRenderer IBocSelectorColumnRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new BocSelectorColumnQuirksModeRenderer (context, list, _bocListQuirksModeCssClassDefinition);
    }
  }
}