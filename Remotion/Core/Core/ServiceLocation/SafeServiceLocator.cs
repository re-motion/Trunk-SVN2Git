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
using Microsoft.Practices.ServiceLocation;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// <see cref="SafeServiceLocator"/> is intended as a wrapper for <see cref="ServiceLocator"/>, specifically the <see cref="ServiceLocator.Current"/>
  /// property. 
  /// </summary>
  /// <remarks>
  /// Accessing <see cref="ServiceLocator.Current"/> will always lead to a <see cref="NullReferenceException"/> if no service locator is 
  /// configured. Using <see cref="SafeServiceLocator.Current"/> instead will catch the exception and set <see cref="ServiceLocator.Current"/>
  /// to the <see cref="NullServiceLocator"/>, which is save for multi-valued operations, thos allowing the use of <see cref="IServiceLocator"/>
  /// in scenarios where IoC provides optional features, e.g. extensions, listeners, etc.
  /// </remarks>
  public static class SafeServiceLocator
  {
    public static IServiceLocator Current
    {
      get {
        try
        {
          return ServiceLocator.Current;
        }
        catch (NullReferenceException)
        {
          ServiceLocator.SetLocatorProvider (() => NullServiceLocator.Instance);
          return NullServiceLocator.Instance;
        }
      }
    }
  }
}