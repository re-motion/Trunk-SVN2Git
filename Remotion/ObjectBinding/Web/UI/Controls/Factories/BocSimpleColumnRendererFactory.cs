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

namespace Remotion.ObjectBinding.Web.UI.Controls.Factories
{
  public class BocSimpleColumnRendererFactory : IBocSimpleColumnRendererFactory
  {
    readonly BocListCssClassDefinition _bocListCssClassDefinition;

    public BocSimpleColumnRendererFactory (BocListCssClassDefinition bocListCssClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("bocListCssClassDefinition", bocListCssClassDefinition);

      _bocListCssClassDefinition = bocListCssClassDefinition;
    }

    public IBocColumnRenderer CreateRenderer (HttpContextBase context, IBocList list, BocSimpleColumnDefinition columnDefinition, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      return new BocSimpleColumnRenderer (
          context, list, columnDefinition, serviceLocator.GetInstance<IResourceUrlFactory> (), _bocListCssClassDefinition);
    }
  }
}