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
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
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
  /// The <see cref="DefaultServiceLocator"/> also provides a set of <see cref="DefaultServiceLocator.Register(ServiceConfigurationEntry)"/> methods 
  /// that allow to registration of custom 
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
  /// <see cref="StackOverflowException"/> or infinite loop. Use the <see cref="Register(ServiceConfigurationEntry)"/> method to manually 
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

    private readonly IDataStore<Type, Func<object>[]> _dataStore = DataStoreFactory.CreateWithLocking<Type, Func<object>[]> ();

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ConcreteImplementationAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
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
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
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
    /// <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instances: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      return GetOrCreateFactories (serviceType).Select (factory => SafeInvokeInstanceFactory (factory, serviceType));
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ConcreteImplementationAttribute"/>, 
    /// or a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    ///<typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ConcreteImplementationAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation 
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> ()
    {
      return (TService) GetInstance (typeof (TService));
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ConcreteImplementationAttribute"/>,
    /// or a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
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
    /// <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instances: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<TService> GetAllInstances<TService> ()
    {
      return GetAllInstances (typeof (TService)).Cast<TService> ();
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ConcreteImplementationAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, 
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
    /// Registers factories for the specified <paramref name="serviceType"/>. 
    /// The factories are subsequently invoked whenever instances for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register the factories for.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <paramref name="serviceType"/>. These factories
    /// must return non-null instances implementing the <paramref name="serviceType"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <paramref name="serviceType"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <paramref name="serviceType"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, params Func<object>[] instanceFactories)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactories", instanceFactories);

      Register (serviceType, (IEnumerable<Func<object>>) instanceFactories);
    }

    /// <summary>
    /// Registers factories for the specified <paramref name="serviceType"/>. 
    /// The factories are subsequently invoked whenever instances for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register the factories for.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <paramref name="serviceType"/>. These factories
    /// must return non-null instances implementing the <paramref name="serviceType"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <paramref name="serviceType"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <paramref name="serviceType"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, IEnumerable<Func<object>> instanceFactories)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactories", instanceFactories);

      var factoryArray = instanceFactories.ToArray();
      var factories = _dataStore.GetOrCreateValue (serviceType, t => factoryArray);
      if (factories != factoryArray)
      {
        var message = string.Format ("Register cannot be called twice or after GetInstance for service type: '{0}'.", serviceType.Name);
        throw new InvalidOperationException (message);
      }
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

      var serviceImplemetation = new ServiceImplementationInfo (concreteImplementationType, lifetime);
      ServiceConfigurationEntry serviceConfigurationEntry;
      try
      {
        serviceConfigurationEntry = new ServiceConfigurationEntry (serviceType, serviceImplemetation);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("Implementation type must implement service type.", "concreteImplementationType", ex);
      }

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

      var factories = CreateInstanceFactories (serviceConfigurationEntry);
      Register (serviceConfigurationEntry.ServiceType, factories);
    }

    private Func<Object>[] GetOrCreateFactories (Type serviceType)
    {
      return _dataStore.GetOrCreateValue (serviceType, t => CreateInstanceFactories (t).ToArray ());
    }

    private object GetInstanceOrNull (Type serviceType)
    {
      var factories = GetOrCreateFactories (serviceType);
      if (factories.Length > 1)
      {
        var message = string.Format (
          "Multiple implemetations are configured for service type: '{0}'. Consider using 'GetAllInstances'.", serviceType.Name);
        throw new ActivationException (message);
      }
      if (factories.Length == 0)
        return null;

      var factory = factories.Single();
      return SafeInvokeInstanceFactory (factory, serviceType);
    }

    private object SafeInvokeInstanceFactory (Func<object> factory, Type serviceType)
    {
      // TODO 4396: Check that result implements serviceType
      object instance;
      try
      {
        instance = factory ();
      }
      catch (Exception ex)
      {
        var message = string.Format ("{0}: {1}", ex.GetType ().Name, ex.Message);
        throw new ActivationException (message, ex);
      }

      if (instance == null)
      {
        var message = string.Format (
            "The registered factory returned null instead of an instance implementing the requested service type '{0}'.",
            serviceType);
        throw new ActivationException (message);
      }

      if (!serviceType.IsInstanceOfType (instance))
      {
        var message = string.Format (
            "The instance returned by the registered factory does not implement the requested type '{0}'. (Instance type: '{1}'.)",
            serviceType,
            instance.GetType());
        throw new ActivationException (message);
      }

      return instance;
    }

    private IEnumerable<Func<object>> CreateInstanceFactories (Type serviceType)
    {
      var attributes = AttributeUtility.GetCustomAttributes<ConcreteImplementationAttribute> (serviceType, false);
      ServiceConfigurationEntry entry;
      try
      {
        entry = ServiceConfigurationEntry.CreateFromAttributes (serviceType, attributes);
      }
      catch (InvalidOperationException ex)
      {
        var message = string.Format ("Invalid ConcreteImplementationAttribute configuration for service type '{0}'. {1}", serviceType, ex.Message);
        throw new ActivationException (message, ex);
      }
      return CreateInstanceFactories (entry);
    }

    private IEnumerable<Func<object>> CreateInstanceFactories (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      return serviceConfigurationEntry.ImplementationInfos.Select (CreateInstanceFactory);
    }

    private Func<object> CreateInstanceFactory (ServiceImplementationInfo serviceImplementationInfo)
    {
      var publicCtors = serviceImplementationInfo.ImplementationType.GetConstructors();
      if (publicCtors.Length != 1)
      {
        throw new ActivationException (
            string.Format (
                "Type '{0}' has not exactly one public constructor and cannot be instantiated.", serviceImplementationInfo.ImplementationType.Name));
      }

      var ctorInfo = publicCtors.Single();
      Func<object> factory = CreateInstanceFactory (ctorInfo);

      switch (serviceImplementationInfo.Lifetime)
      {
        case LifetimeKind.Singleton:
          var factoryContainer = new DoubleCheckedLockingContainer<object> (factory);
          return () => factoryContainer.Value;
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