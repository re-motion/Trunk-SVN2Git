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
  /// a <see cref="NullReferenceException"/> but instead register an default <see cref="IServiceLocator"/> instance if no custom service locator was
  /// registered.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Accessing <see cref="ServiceLocator"/> will always lead to a <see cref="NullReferenceException"/> if no service locator is 
  /// configured. Using <see cref="SafeServiceLocator"/> instead will catch the exception and register a default <see cref="IServiceLocator"/> instance.
  /// A provider for the default instance can be defined in the application configuration file (handled by 
  /// <see cref="ServiceLocationConfiguration"/>). The provider needs to implement <see cref="IServiceLocatorProvider"/> and must have a default 
  /// constructor.
  /// <code>
  /// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
  /// &lt;configuration&gt;
  ///   &lt;configSections&gt;
  ///     &lt;section name="remotion.serviceLocation" type="Remotion.Configuration.ServiceLocation.ServiceLocationConfiguration,Remotion" /&gt;
  ///   &lt;/configSections&gt;
  /// 
  ///   &lt;remotion.serviceLocation xmlns="http://www.re-motion.org/serviceLocation/configuration"&gt;
  ///     &lt;serviceLocatorProvider type="MyAssembly::MyServiceLocatorProvider"/&gt;
  ///   &lt;/remotion.serviceLocation&gt;
  /// &lt;/configuration&gt;
  /// </code>
  /// </para>
  /// <para>
  /// If no provider is configured, a <see cref="DefaultServiceLocator"/> instance is used as the default instance.
  /// </para>
  /// </remarks>
  public static class SafeServiceLocator
  {
    private static readonly BootstrapServiceConfiguration s_bootstrapServiceConfiguration = new BootstrapServiceConfiguration();

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

    /// <summary>
    /// Allows clients to register services that are available while the default <see cref="IServiceLocator"/> is built.
    /// These service registrations are also included in the default configuration returned by the 
    /// <see cref="DefaultServiceConfigurationDiscoveryService"/> (TODO 5396).
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the <see cref="Current"/> property is accessed for the first time, and no <see cref="IServiceLocatorProvider"/> provider has been set 
    /// via <see cref="ServiceLocator.SetLocatorProvider"/>, a default <see cref="IServiceLocator"/> is built as defined by the 
    /// <see cref="ServiceLocationConfiguration"/>. Building the default provider, which may include construction of an IoC container, can be a
    /// complex operation. when the code building the default provider accesses the <see cref="Current"/> property, it will get a reference to a
    /// bootstrapping <see cref="IServiceLocator"/>, which behaves just like the <see cref="DefaultServiceLocator"/> (i.e., it evaluates
    /// <see cref="ConcreteImplementationAttribute"/> declarations and such).
    /// The <see cref="BootstrapConfiguration"/> allows clients to register additional services with the bootstrapping <see cref="IServiceLocator"/>.
    /// </para>
    /// </remarks>
    public static IBootstrapServiceConfiguration BootstrapConfiguration
    {
      get { return s_bootstrapServiceConfiguration; }
    }

    private static IServiceLocator GetDefaultServiceLocator ()
    {
      // Temporarily set the bootstrapper to allow for reentrancy to SafeServiceLocator.Current.
      // Since we're called from s_defaultServiceLocator.Value's getter, we can be sure that our return value will overwrite the bootstrapper.
      s_defaultServiceLocator.Value = s_bootstrapServiceConfiguration.BootstrapServiceLocator;

      var serviceLocatorProvider = ServiceLocationConfiguration.Current.CreateServiceLocatorProvider();
      return serviceLocatorProvider.GetServiceLocator();
    }
  }
}