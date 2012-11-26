// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Remotion.Configuration.ServiceLocation;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// <see cref="SafeServiceLocator"/> is intended as a wrapper for <see cref="ServiceLocator"/>, specifically the 
  /// <see cref="ServiceLocator.Current"/> property. In contrast to <see cref="ServiceLocator"/>, <see cref="SafeServiceLocator"/> will never throw
  /// a <see cref="NullReferenceException"/> but instead register an instance of the DefaultServiceLocatorClass if no custom service locator was
  /// registered.
  /// </summary>
  /// <remarks>
  /// Accessing <see cref="ServiceLocator"/> will always lead to a <see cref="NullReferenceException"/> if no service locator is 
  /// configured. Using <see cref="SafeServiceLocator"/> instead will catch the exception and register an instance of the DefaultServiceLocator class.
  /// </remarks>
  public static class SafeServiceLocator
  {
    // This is a DoubleCheckedLockingContainer rather than a static field (maybe wrapped in a nested class to improve laziness) because we want
    // any exceptions thrown by GetDefaultServiceLocator to bubble up to the caller normally. (Exceptions during static field initialization get
    // wrapped in a TypeInitializationException.)
    private static readonly DoubleCheckedLockingContainer<IServiceLocator> s_defaultServiceLocator = 
        new DoubleCheckedLockingContainer<IServiceLocator> (GetDefaultServiceLocator);
    
    /// <summary>
    /// Gets the currently configured <see cref="IServiceLocator"/>. 
    /// If no service locator is configured or <see cref="ServiceLocator.Current"/> returns <see langword="null" />, 
    /// an <see cref="IServiceLocator"/> will be returned.
    /// </summary>
    public static IServiceLocator Current
    {
      // Have debugger step through to avoid breaking on the NullReferenceException that might be thrown below.
      [DebuggerStepThrough]
      get
      {
        try
        {
          return ServiceLocator.Current ?? s_defaultServiceLocator.Value;
        }
        catch (NullReferenceException)
        {
          ServiceLocator.SetLocatorProvider (() => s_defaultServiceLocator.Value);
          return s_defaultServiceLocator.Value;
        }
      }
    }

    private static IServiceLocator GetDefaultServiceLocator ()
    {
      var serviceLocatorProvider = ServiceLocationConfiguration.Current.CreateServiceLocatorProvider();
      return serviceLocatorProvider.GetServiceLocator ();
    }
  }
}