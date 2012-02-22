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

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Holds the parameters used by <see cref="DefaultServiceLocator"/> for instantiating instances of service types. Use 
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> to retrieve the <see cref="ServiceConfigurationEntry"/> data for a specific type.
  /// </summary>
  public class ServiceConfigurationEntry
  {
    /// <summary>
    /// Encapsulates a service implementation type and <see cref="LifetimeKind"/>.
    /// </summary>
    public struct ServiceImplementationInfo
    {
      private readonly Type _implementationType;
      private readonly LifetimeKind _lifetime;

      /// <summary>
      /// Initializes a new instance of the <see cref="ServiceImplementationInfo"/> struct.
      /// </summary>
      /// <param name="implementationType">The concrete implementation of the service type.</param>
      /// <param name="lifetime">The lifetime of the instances of <paramref name="implementationType"/>.</param>
      public ServiceImplementationInfo (Type implementationType, LifetimeKind lifetime)
      {
        _implementationType = implementationType;
        _lifetime = lifetime;
      }

      /// <summary>
      /// Gets the concrete implementation of the <see cref="ServiceType"/>.
      /// </summary>
      /// <value>The concrete implementation.</value>
      public Type ImplementationType
      {
        get { return _implementationType; }
      }

      /// <summary>
      /// Gets the lifetime of the instances of <see cref="ImplementationType"/>.
      /// </summary>
      /// <value>The lifetime of the instances.</value>
      public LifetimeKind Lifetime
      {
        get { return _lifetime; }
      }
    }

    private readonly Type _serviceType;
    private readonly ServiceImplementationInfo _serviceImplementationInfo;

    /// <summary>
    /// Creates a <see cref="ServiceConfigurationEntry"/> from a <see cref="ConcreteImplementationAttribute"/>.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="attribute">The attribute holding information about the concrete implementation of the <paramref name="serviceType"/>.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> containing the data from the <paramref name="attribute"/>.</returns>
    public static ServiceConfigurationEntry CreateFromAttribute (Type serviceType, ConcreteImplementationAttribute attribute)
    {
      var serviceImplementation = new ServiceImplementationInfo (TypeNameTemplateResolver.ResolveToType (attribute.TypeNameTemplate), attribute.Lifetime);
      return new ServiceConfigurationEntry (serviceType, serviceImplementation);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceConfigurationEntry"/> class.
    /// </summary>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="serviceImplementationInfo">The service implementation information.</param>
    public ServiceConfigurationEntry (Type serviceType, ServiceImplementationInfo serviceImplementationInfo)
    {
      _serviceType = serviceType;
      _serviceImplementationInfo = serviceImplementationInfo;
    }

    /// <summary>
    /// Gets the service type. This is a type for which instances are requested from a service locator.
    /// </summary>
    /// <value>The service type.</value>
    public Type ServiceType
    {
      get { return _serviceType; }
    }

    /// <summary>
    /// Gets information about the service implementation.
    /// </summary>
    /// <value>The <see cref="ServiceImplementationInfo"/>.</value>
    public ServiceImplementationInfo ImplementationInfo
    {
      get { return _serviceImplementationInfo; }
    }
  }
}