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
    // This class holds lazily initialized, readonly static fields. It relies on the fact that the .NET runtime will reliably initialize fields in a 
    // nested static class with a static constructor as lazily as possible on first access of the static field.
    // Singleton implementations with nested classes are documented here: http://csharpindepth.com/Articles/General/Singleton.aspx.
    static class LazyStaticFields
    {
      public static readonly IServiceLocator DefaultServiceLocatorInstance = new DefaultServiceLocator();

      // ReSharper disable EmptyConstructor
      // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit; this will make the static fields as lazy as possible.
      static LazyStaticFields ()
      {
      }
      // ReSharper restore EmptyConstructor
    }
    
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
          return ServiceLocator.Current ?? LazyStaticFields.DefaultServiceLocatorInstance;
        }
        catch (NullReferenceException)
        {
          ServiceLocator.SetLocatorProvider (() => LazyStaticFields.DefaultServiceLocatorInstance);
          return LazyStaticFields.DefaultServiceLocatorInstance;
        }
      }
    }
  }
}