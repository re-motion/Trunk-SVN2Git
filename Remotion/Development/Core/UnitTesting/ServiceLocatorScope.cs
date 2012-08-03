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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Sets <see cref="ServiceLocator.Current"/> to a given <see cref="IServiceLocator"/> temporarily, resetting it, when <see cref="Dispose"/>
  /// is called.
  /// </summary>
  public class ServiceLocatorScope : IDisposable
  {
    private static DefaultServiceLocator CreateServiceLocator (IEnumerable<ServiceConfigurationEntry> configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      var defaultServiceLocator = new DefaultServiceLocator ();
      foreach (var stubbedRegistration in configuration)
        defaultServiceLocator.Register (stubbedRegistration);
      return defaultServiceLocator;
    }

    private static DefaultServiceLocator CreateServiceLocator (Type serviceType, Func<object> creator)
    {
      var defaultServiceLocator = CreateServiceLocator (Enumerable.Empty<ServiceConfigurationEntry>());
      defaultServiceLocator.Register (serviceType, creator);
      return defaultServiceLocator;
    }

    private readonly ServiceLocatorProvider _previousLocatorProvider;

    public ServiceLocatorScope (IServiceLocator temporaryServiceLocator)
    {
      try
      {
        var previousLocator = ServiceLocator.Current;
        _previousLocatorProvider = () => previousLocator;
      }
      catch (NullReferenceException)
      {
        _previousLocatorProvider = null;
      }

      ServiceLocator.SetLocatorProvider (() => temporaryServiceLocator);
    }

    public ServiceLocatorScope (params ServiceConfigurationEntry[] temporaryConfiguration)
      : this (CreateServiceLocator (temporaryConfiguration))
    {
    }

    public ServiceLocatorScope (Type serviceType, Type implementationType, LifetimeKind lifetimeKind = LifetimeKind.Instance)
        : this (
            CreateServiceLocator (
                new[] { new ServiceConfigurationEntry (serviceType, new ServiceImplementationInfo (implementationType, lifetimeKind)) }))
    {
    }

    public ServiceLocatorScope (Type serviceType, Func<object> creator)
      : this (CreateServiceLocator (serviceType, creator))
    {
    }

    public void Dispose ()
    {
      ServiceLocator.SetLocatorProvider (_previousLocatorProvider);
    }
  }
}