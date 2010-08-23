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
using Remotion.Implementation;
using Remotion.Web.UI.Controls.Factories;

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  /// <summary>
  /// Defines a factory method for creating renderers for <see cref="IWebTabStrip"/> controls.
  /// </summary>
  [ConcreteImplementation (typeof(WebTabStripRendererFactory), LifeTime = LifetimeKind.Singleton)]
  public interface IWebTabStripRendererFactory
  {
    IRenderer CreateRenderer (HttpContextBase context, IWebTabStrip control, IServiceLocator serviceLocator);
  }
}