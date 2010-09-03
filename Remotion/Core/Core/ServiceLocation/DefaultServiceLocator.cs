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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.Implementation;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides a default implementation of the <see cref="IServiceLocator"/> interface based on the <see cref="ConcreteImplementationAttribute"/>.
  /// The <see cref="SafeServiceLocator"/> uses (and registers) an instance of this class unless an application registers its own service locator via 
  /// <see cref="ServiceLocator.SetLocatorProvider"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This implementation of <see cref="IServiceLocator"/> uses the <see cref="ConcreteImplementationAttribute"/> to resolve implementations of
  /// "service types" (usually interfaces or abstract classes). When the <see cref="DefaultServiceLocator"/> is asked to get an instance of a specific 
  /// service type for the first time, that type is checked for a <see cref="ConcreteImplementationAttribute"/>, which is then inspected to determine 
  /// the actual concrete type to be instantiated, its lifetime, and similar properties. An instance is then returned that fulfills the properties 
  /// defined by the <see cref="ConcreteImplementationAttribute"/>. After the first resolution of a service type, the instance (or a factory, 
  /// depending on the <see cref="LifetimeKind"/> associated with the type) is cached, so subsequent lookups for the same type are very fast.
  /// </para>
  /// <para>
  /// The <see cref="DefaultServiceLocator"/> also provides a set of <see cref="O:Register"/> methods that allow to registration of custom 
  /// implementations or factories for service types even if those types do not have the <see cref="ConcreteImplementationAttribute"/> applied. 
  /// Applications can use this to override the configuration defined by the <see cref="ConcreteImplementationAttribute"/> and to register 
  /// implementations of service types that do not have the <see cref="ConcreteImplementationAttribute"/> applied. Custom implementations or factories
  /// must be registered before an instance of the respective service type is retrieved for the first time.
  /// </para>
  /// <para>
  /// In order to be instantiable by the <see cref="DefaultServiceLocator"/>, a concrete type indicated by the 
  /// <see cref="ConcreteImplementationAttribute"/> must have exactly one public constructor. The constructor may have parameters, in which case
  /// the <see cref="DefaultServiceLocator"/> will try to get an instance for each of the parameters using the same <see cref="IServiceLocator"/>
  /// methods. If a parameter cannot be resolved (because the parameter type has no <see cref="ConcreteImplementationAttribute"/> applied and no
  /// custom implementation or factory was manually registered), an exception is thrown. Dependency cycles are not detected and will lead to a 
  /// <see cref="StackOverflowException"/> or infinite loop. Use the <see cref="Register(System.Type,System.Func{object})"/> method to manually 
  /// register a factory for types that do not apply to these constructor rules.
  /// </para>
  /// <para>
  /// In order to have a custom service locator use the same defaults as the <see cref="DefaultServiceLocator"/>, the 
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> can be used to extract those defaults from a set of types.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public class DefaultServiceLocator : IServiceLocator
  {
    private static readonly MethodInfo s_genericGetInstanceMethod = typeof (IServiceLocator).GetMethod ("GetInstance", Type.EmptyTypes);
    
    private readonly InterlockedCache<Type, Func<object>> _cache = new InterlockedCache<Type, Func<object>>();

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ConcreteImplementationAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="O:Register"/> methods.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ConcreteImplementationAttribute"/> could not be found on the <paramref name="serviceType"/>, or the concrete implementation could
    /// not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public object GetInstance (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      var instance = GetInstanceOrNull (serviceType);
      if (instance == null)
      {
        string message = string.Format (
            "Cannot get a concrete implementation of type '{0}': Expected 'ConcreteImplementationAttribute' could not be found.",
            serviceType.FullName);
        throw new ActivationException (message);
      }

      return instance;
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ConcreteImplementationAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="O:Register"/> methods.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <param name="key">The name the object was registered with. This parameter is ignored by this implementation of <see cref="IServiceLocator"/>.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ConcreteImplementationAttribute"/> could not be found on the <paramref name="serviceType"/>, or the concrete implementation could
    /// not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public object GetInstance (Type serviceType, string key)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      return GetInstance (serviceType);
    }

    /// <summary>
    /// Get all instance of the given <paramref name="serviceType"/>, or an empty sequence if no instance could be found.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>
    /// A sequence of instances of the requested <paramref name="serviceType"/>. The <paramref name="serviceType"/> must either have a 
    /// <see cref="ConcreteImplementationAttribute"/>, or a concrete implementation or factory must have been registered using one of the 
    /// <see cref="O:Register"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instances: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      var instance = GetInstanceOrNull (serviceType);
      if (instance != null)
        return new[] { instance };
      else
        return new object[0];
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ConcreteImplementationAttribute"/>, 
    /// or a concrete implementation or factory must have been registered using one of the <see cref="O:Register"/> methods.
    /// </summary>
    ///<typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ConcreteImplementationAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation 
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> ()
    {
      try
      {
        return (TService) GetInstance (typeof (TService));
      }
      catch (InvalidCastException ex)
      {
        var message = "The implementation type does not implement the service interface. " + ex.Message;
        throw new ActivationException (message, ex);
      }
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ConcreteImplementationAttribute"/>,
    /// or a concrete implementation or factory must have been registered using one of the <see cref="O:Register"/> methods.
    /// </summary>
    /// <typeparam name="TService">The type of object requested.</typeparam>
    /// <param name="key">The name the object was registered with. This parameter is ignored by this implementation of <see cref="IServiceLocator"/>.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The
    /// <see cref="ConcreteImplementationAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> (string key)
    {
      return (TService) GetInstance (typeof (TService), key);
    }

    /// <summary>
    /// Get all instance of the given <typeparamref name="TService"/> type, or an empty sequence if no instance could be found.
    /// </summary>
    /// <typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>
    /// A sequence of instances of the requested <typeparamref name="TService"/> type. The <typeparamref name="TService"/> type must either have a 
    /// <see cref="ConcreteImplementationAttribute"/>, or a concrete implementation or factory must have been registered using one of the 
    /// <see cref="O:Register"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instances: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<TService> GetAllInstances<TService> ()
    {
      var instance = GetInstanceOrNull (typeof (TService));
      if (instance != null)
        return new[] { (TService) instance };
      else
        return new TService[0];
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ConcreteImplementationAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="O:Register"/> methods. Otherwise, 
    /// the method returns <see langword="null"/>.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>The requested service instance, or <see langword="null" /> if no instance could be found.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    object IServiceProvider.GetService (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      return GetInstanceOrNull (serviceType);
    }

    /// <summary>
    /// Registers a factory for the specified <paramref name="serviceType"/>. 
    /// The factory is subsequently invoked whenever an instance for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register a factory for.</param>
    /// <param name="instanceFactory">The instance factory to use when resolving an instance for the <paramref name="serviceType"/>.</param>
    /// <exception cref="InvalidOperationException">An instance of the <paramref name="serviceType"/> has already been retrieved. Registering factories
    /// or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, Func<object> instanceFactory)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactory", instanceFactory);

      Func<object> factory = _cache.GetOrCreateValue (serviceType, t => instanceFactory);
      if (factory != instanceFactory)
        throw new InvalidOperationException (string.Format ("Register cannot be called after GetInstance for service type: {0}", serviceType.Name));
    }

    /// <summary>
    /// Registers a concrete implementation for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The service type to register a concrete implementation for.</param>
    /// <param name="concreteImplementationType">The type of the concrete implementation to be instantiated when an instance of the 
    /// <paramref name="serviceType"/> is retrieved.</param>
    /// <param name="lifetime">The lifetime of the instances.</param>
    /// <exception cref="InvalidOperationException">An instance of the <paramref name="serviceType"/> has already been retrieved. Registering factories
    /// or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, Type concreteImplementationType, LifetimeKind lifetime)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("concreteImplementationType", concreteImplementationType);

      var serviceConfigurationEntry = new ServiceConfigurationEntry (serviceType, concreteImplementationType, lifetime);
      Register (serviceConfigurationEntry);
    }

    /// <summary>
    /// Registers a concrete implementation.
    /// </summary>
    /// <param name="serviceConfigurationEntry">A <see cref="ServiceConfigurationEntry"/> describing the concrete implementation to be registered.</param>
    /// <exception cref="InvalidOperationException">An instance of the service type described by the <paramref name="serviceConfigurationEntry"/>
    /// has already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      ArgumentUtility.CheckNotNull ("serviceConfigurationEntry", serviceConfigurationEntry);

      var factory = CreateInstanceFactory (serviceConfigurationEntry);
      Register (serviceConfigurationEntry.ServiceType, factory);
    }

    private object GetInstanceOrNull (Type serviceType)
    {
      var factory = _cache.GetOrCreateValue (serviceType, CreateInstanceFactory);
      try
      {
        return factory ();
      }
      catch (Exception ex)
      {
        var message = string.Format ("{0} : {1}", ex.GetType ().FullName, ex.Message);
        throw new ActivationException (message, ex);
      }
    }

    private Func<object> CreateInstanceFactory (Type serviceType)
    {
      var concreteImplementationAttribute = AttributeUtility.GetCustomAttribute<ConcreteImplementationAttribute> (serviceType, false);
      if (concreteImplementationAttribute == null)
        return () => null;

      var serviceConfigurationEntry = ServiceConfigurationEntry.CreateFromAttribute (serviceType, concreteImplementationAttribute);
      return CreateInstanceFactory (serviceConfigurationEntry);
    }

    private Func<object> CreateInstanceFactory (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      var publicCtors = serviceConfigurationEntry.ImplementationType.GetConstructors().Where (ci => ci.IsPublic).ToArray();
      if (publicCtors.Length != 1)
      {
        throw new ActivationException (
            string.Format (
                "Type '{0}' has not exactly one public constructor and cannot be instantiated.", serviceConfigurationEntry.ImplementationType.Name));
      }

      var ctorInfo = publicCtors[0];
      Func<object> factory = CreateInstanceFactory (ctorInfo);

      switch (serviceConfigurationEntry.Lifetime)
      {
        case LifetimeKind.Singleton:
          var instance = factory();
          return () => instance;

        default:
          return factory;
      }
    }

    private Func<object> CreateInstanceFactory (ConstructorInfo ctorInfo)
    {
      var serviceLocator = Expression.Constant (this);

      var parameterInfos = ctorInfo.GetParameters();
      var ctorArgExpressions = 
          parameterInfos.Select (p => (Expression) Expression.Call (serviceLocator, s_genericGetInstanceMethod.MakeGenericMethod (p.ParameterType)));

      var factoryLambda = Expression.Lambda<Func<object>> (Expression.New (ctorInfo, ctorArgExpressions));
      return factoryLambda.Compile();
    }
  }
}