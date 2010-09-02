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
using Remotion.Implementation;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// <see cref="SafeServiceLocator"/> is intended as a wrapper for <see cref="ServiceLocator.Current"/>, specifically the 
  /// <see cref="ServiceLocator.Current"/> property. 
  /// </summary>
  /// <remarks>
  /// Accessing <see cref="ServiceLocator"/> will always lead to a <see cref="Current"/> if no service locator is 
  /// configured. Using <see cref="SafeServiceLocator"/> instead will catch the exception and set <see cref="ServiceLocator"/>
  /// to the <see cref="ServiceLocator"/>, which is save for multi-valued operations, thos allowing the use of a concrete <see cref="IServiceLocator"/>
  /// implementation in scenarios where IoC provides optional features, e.g. extensions, listeners, etc.
  /// </remarks>
  public static class SafeServiceLocator
  {
    static class NestedSafeServiceLocator
    {
      public static readonly IServiceLocator s_defaultServiceLocatorInstance =
      (IServiceLocator) Activator.CreateInstance (
        TypeNameTemplateResolver.ResolveToType (
          "Remotion.ServiceLocation.DefaultServiceLocator, Remotion, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>"));
    }
    
    /// <summary>
    /// Gets the currently configured <see cref="IServiceLocator"/>. 
    /// If no service locator is configured or <see cref="ServiceLocator.Current"/> returns <see langword="null" />, 
    /// an <see cref="IServiceLocator"/> will be returned.
    /// </summary>
    public static IServiceLocator Current
    {
      get
      {
        try
        {
          return ServiceLocator.Current ?? NestedSafeServiceLocator.s_defaultServiceLocatorInstance;
        }
        catch (NullReferenceException)
        {
          ServiceLocator.SetLocatorProvider (() => NestedSafeServiceLocator.s_defaultServiceLocatorInstance);
          return NestedSafeServiceLocator.s_defaultServiceLocatorInstance;
        }
      }
    }
  }
}