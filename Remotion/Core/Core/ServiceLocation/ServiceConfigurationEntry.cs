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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Holds the parameters used by <see cref="DefaultServiceLocator"/> for instantiating instances of service types. Use 
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> to retrieve the <see cref="ServiceConfigurationEntry"/> data for a specific type.
  /// </summary>
  public class ServiceConfigurationEntry
  {
    private readonly Type _serviceType;
    private readonly ReadOnlyCollection<ServiceImplementationInfo> _implementationInfos;

    /// <summary>
    /// Creates a <see cref="ServiceConfigurationEntry"/> from a <see cref="ConcreteImplementationAttribute"/>.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="attributes">The attributes holding information about the concrete implementation of the <paramref name="serviceType"/>.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> containing the data from the <paramref name="attributes"/>.</returns>
    public static ServiceConfigurationEntry CreateFromAttributes (Type serviceType, IEnumerable<ConcreteImplementationAttribute> attributes)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("attributes", attributes);

      var attributesAndResolvedTypes =
          (from attribute in attributes
           orderby attribute.Position
           let resolvedType = ResolveType(attribute)
           where resolvedType != null
           select new { Attribute = attribute, ResolvedType = resolvedType }).ConvertToCollection();

      EnsureUniqueProperty ("Implementation type", attributesAndResolvedTypes.Select (tuple => tuple.ResolvedType));
      EnsureUniqueProperty ("Position", attributesAndResolvedTypes.Select (tuple => tuple.Attribute.Position));

      var serviceImplementationInfos = 
          attributesAndResolvedTypes.Select (tuple => new ServiceImplementationInfo (tuple.ResolvedType, tuple.Attribute.Lifetime));
      
      return new ServiceConfigurationEntry (serviceType, serviceImplementationInfos);
    }

    private static Type ResolveType (ConcreteImplementationAttribute attribute)
    {
      try
      {
        return TypeNameTemplateResolver.ResolveToType (attribute.TypeNameTemplate);
      }
      catch (FileNotFoundException) // Invalid assembly
      {
        if (attribute.IgnoreIfNotFound)
          return null;
        throw;
      }
      catch (TypeLoadException) // Invalid type name
      {
        if (attribute.IgnoreIfNotFound)
          return null;
        throw;
      }
    }

    private static void EnsureUniqueProperty<T> (string propertyDescription, IEnumerable<T> propertyValues)
    {
      var visitedValues = new HashSet<T> ();
      foreach (var value in propertyValues)
      {
        if (visitedValues.Contains (value))
        {
          var message = string.Format ("Ambiguous {0}: {1} must be unique.", typeof (ConcreteImplementationAttribute).Name, propertyDescription);
          throw new InvalidOperationException (message);
        }
        visitedValues.Add (value);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceConfigurationEntry"/> class.
    /// </summary>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="implementationInfos">The <see cref="ServiceImplementationInfo"/> for the <paramref name="serviceType" />.</param>
    public ServiceConfigurationEntry (
        Type serviceType, params ServiceImplementationInfo[] implementationInfos)
        : this (serviceType, (IEnumerable<ServiceImplementationInfo>) implementationInfos)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceConfigurationEntry"/> class.
    /// </summary>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="implementationInfos">The service implementation information.</param>
    public ServiceConfigurationEntry (Type serviceType, IEnumerable<ServiceImplementationInfo> implementationInfos)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("implementationInfos", implementationInfos);

      _serviceType = serviceType;
      _implementationInfos = Array.AsReadOnly (implementationInfos.ToArray ());
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
    /// Gets information about all service implementations.
    /// </summary>
    /// <value>A collection of <see cref="ServiceImplementationInfo"/> instances.</value>
    /// <remarks>
    /// When this information is used to configure an implementation of <see cref="IServiceLocator"/>, <see cref="IServiceLocator.GetAllInstances"/>
    /// must return the implementing instances in exactly the same order as defined by <see cref="ImplementationInfos"/>.
    /// </remarks>
    public ReadOnlyCollection<ServiceImplementationInfo> ImplementationInfos
    {
      get { return _implementationInfos; }
    }

    /// <inheritdoc />
    public override string ToString ()
    {
      var implementationInfos = _implementationInfos.Select (i => i.ToString()).ToArray();
      var joinedImplementationInfos = string.Join (", ", implementationInfos);
      return string.Format ("{0} implementations: [{1}]", _serviceType, joinedImplementationInfos);
    }
  }
}