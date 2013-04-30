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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides services for scanning a range of types for default service configuration settings, as they would be applied by 
  /// <see cref="DefaultServiceLocator"/>. Use this class in order to configure a specific service locator with <see cref="DefaultServiceLocator"/>'s
  /// defaults.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> uses the same logic as <see cref="DefaultServiceLocator"/> in order to find the
  /// default concrete implementation of service types configured via the <see cref="ConcreteImplementationAttribute"/>. See 
  /// <see cref="DefaultServiceLocator"/> for more information about this.
  /// </para>
  /// <para>
  /// Concrete implementations registered with a specific <see cref="DefaultServiceLocator"/> using its <see cref="DefaultServiceLocator.Register(ServiceConfigurationEntry)"/>
  /// methods are not returned by this class.
  /// </para>
  /// </remarks>
  public static class DefaultServiceConfigurationDiscoveryService
  {
    /// <summary>
    /// Gets the default service configuration for the types returned by the given <see cref="ITypeDiscoveryService"/>.
    /// </summary>
    /// <param name="typeDiscoveryService">The type discovery service.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type returned by the <paramref name="typeDiscoveryService"/> that has the
    /// <see cref="ConcreteImplementationAttribute"/> applied. Types without the attribute are ignored.</returns>
    public static IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      return GetDefaultConfiguration (typeDiscoveryService.GetTypes (null, false).Cast<Type>());
    }

    /// <summary>
    /// Gets the default service configuration for the given types.
    /// </summary>
    /// <param name="types">The types to get the default service configuration for.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type that has the <see cref="ConcreteImplementationAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public static IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return (from type in types
              let concreteImplementationAttributes = AttributeUtility.GetCustomAttributes<ConcreteImplementationAttribute> (type, false)
              where concreteImplementationAttributes.Length != 0
              select CreateServiceConfigurationEntry (type, concreteImplementationAttributes));
    }

    /// <summary>
    /// Gets the default service configuration for the types in the given assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies for whose types to get the default service configuration.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type that has the <see cref="ConcreteImplementationAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public static IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SelectMany (a => GetDefaultConfiguration (a.GetTypes()));
    }

    private static ServiceConfigurationEntry CreateServiceConfigurationEntry (Type type, ConcreteImplementationAttribute[] concreteImplementationAttributes)
    {
      try
      {
        return ServiceConfigurationEntry.CreateFromAttributes (type, concreteImplementationAttributes);
      }
      catch (InvalidOperationException ex)
      {
        var message = string.Format ("Invalid configuration of service type '{0}'. {1}", type, ex.Message);
        throw new InvalidOperationException (message, ex);
      }
    }
  }
}