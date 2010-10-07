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
          IBocCustomColumnRendererFactory,
          IBocDropDownMenuColumnRendererFactory,
          IBocRowEditModeColumnRendererFactory
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
      return new BocSimpleColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCompoundColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocCompoundColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocCustomColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocCustomColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocDropDownMenuColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocDropDownMenuColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
    }

    public IBocColumnRenderer CreateRenderer (
        HttpContextBase context, IBocList list, BocRowEditModeColumnDefinition columnDefinition, IServiceLocator serviceLocator, int columnIndex)
    {
      return new BocRowEditModeColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
    }
    
  }
}