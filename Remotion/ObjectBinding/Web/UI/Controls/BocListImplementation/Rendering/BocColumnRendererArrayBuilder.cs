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
using System.Collections.Generic;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  public class BocColumnRendererArrayBuilder
  {
    private readonly BocColumnDefinition[] _columnDefinitions;
    private readonly IServiceLocator _serviceLocator;

    public BocColumnRendererArrayBuilder (BocColumnDefinition[] columnDefinitions, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnDefinitions", columnDefinitions);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      _columnDefinitions = columnDefinitions;
      _serviceLocator = serviceLocator;
    }

    public BocColumnRenderer[] CreateColumnRenderers (HttpContextBase context, IBocList list) //TODO: remove these parameters as soon as the renderers are stateless
    {
      var bocColumnRenderers = new List<BocColumnRenderer>(_columnDefinitions.Length);
      for (int columnIndex = 0; columnIndex < _columnDefinitions.Length; columnIndex++)
      {
        var columnDefinition = _columnDefinitions[columnIndex];
        var columnRenderer = columnDefinition.GetRenderer (_serviceLocator, context, list, columnIndex);
        bocColumnRenderers.Add(new BocColumnRenderer (columnRenderer, columnDefinition, columnIndex));
      }
      return bocColumnRenderers.ToArray();
    }
  }
}