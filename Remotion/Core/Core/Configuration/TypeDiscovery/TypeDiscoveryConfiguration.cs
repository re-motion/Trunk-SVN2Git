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
using System.ComponentModel.Design;
using System.Configuration;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures the type discovery performed by <see cref="ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService"/>.
  /// </summary>
  public sealed class TypeDiscoveryConfiguration : ConfigurationSection
  {
    private static readonly DoubleCheckedLockingContainer<TypeDiscoveryConfiguration> s_current = 
        new DoubleCheckedLockingContainer<TypeDiscoveryConfiguration> (GetTypeDiscoveryConfiguration);

    /// <summary>
    /// Gets the current <see cref="TypeDiscoveryConfiguration"/> instance. This is used by 
    /// <see cref="ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService"/> to retrieve a <see cref="ITypeDiscoveryService"/> instance if
    /// <see cref="DesignerUtility.IsDesignMode"/> is not set to <see langword="true" /> and no specific 
    /// <see cref="ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService"/> instance has been set.
    /// </summary>
    /// <value>The current <see cref="TypeDiscoveryConfiguration"/>.</value>
    public static TypeDiscoveryConfiguration Current
    {
      get { return s_current.Value; }
    }

    /// <summary>
    /// Sets the <see cref="Current"/> <see cref="TypeDiscoveryConfiguration"/> instance.
    /// </summary>
    /// <param name="configuration">The new configuration to set as the <see cref="Current"/> configuration.</param>
    public static void SetCurrent (TypeDiscoveryConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    private static TypeDiscoveryConfiguration GetTypeDiscoveryConfiguration ()
    {
      return (TypeDiscoveryConfiguration) 
             ConfigurationWrapper.Current.GetSection ("remotion.typeDiscovery", false) ?? new TypeDiscoveryConfiguration();
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="TypeDiscoveryConfiguration"/> class. To load the configuration from a config file,
    /// use <see cref="ConfigurationWrapper.GetSection(string)"/> instead.
    /// </summary>
    public TypeDiscoveryConfiguration ()
    {
      var xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      Properties.Add (xmlnsProperty);
    }

    /// <summary>
    /// Gets or sets the <see cref="TypeDiscoveryMode"/> to be used for type discovery.
    /// </summary>
    /// <value>The <see cref="TypeDiscoveryMode"/> to be used for type discovery.</value>
    [ConfigurationProperty ("mode", DefaultValue = TypeDiscoveryMode.Automatic, IsRequired = false)]
    public TypeDiscoveryMode Mode
    {
      get { return (TypeDiscoveryMode) this["mode"]; }
      set { this["mode"] = value; }
    }

    /// <summary>
    /// Gets a <see cref="TypeElement{TBase}"/> describing the custom <see cref="IRootAssemblyFinder"/> to be used. This is only relevant
    /// if <see cref="Mode"/> is set to <see cref="TypeDiscoveryMode.CustomRootAssemblyFinder"/>. In this mode, an 
    /// <see cref="AssemblyFinderTypeDiscoveryService"/> is created, and an instance of the specified <see cref="CustomRootAssemblyFinder"/> type is
    /// employed for finding the root assemblies used for type discovery. The given type must have a default constructor.
    /// </summary>
    /// <value>A <see cref="TypeElement{TBase}"/> describing the custom <see cref="IRootAssemblyFinder"/> type to be used.</value>
    [ConfigurationProperty ("customRootAssemblyFinder", IsRequired = false)]
    public TypeElement<IRootAssemblyFinder> CustomRootAssemblyFinder
    {
      get { return (TypeElement<IRootAssemblyFinder>) this["customRootAssemblyFinder"]; }
    }

    /// <summary>
    /// Gets a <see cref="RootAssembliesElement"/> describing specific root assemblies to be used. This is only relevant
    /// if <see cref="Mode"/> is set to <see cref="TypeDiscoveryMode.SpecificRootAssemblies"/>. In this mode, an 
    /// <see cref="AssemblyFinderTypeDiscoveryService"/> is created, and the given root assemblies are employed for type discovery.
    /// Note that even if an assembly is specified as a root assembly, the default filtering rules (<see cref="ApplicationAssemblyLoaderFilter"/>)
    /// still apply even for that assembly.
    /// </summary>
    /// <value>A <see cref="RootAssembliesElement"/> describing specific root assemblies to be used.</value>
    [ConfigurationProperty ("specificRootAssemblies", IsRequired = false)]
    public RootAssembliesElement SpecificRootAssemblies
    {
      get { return (RootAssembliesElement) this["specificRootAssemblies"]; }
    }

    /// <summary>
    /// Gets a <see cref="TypeElement{TBase}"/> describing the custom <see cref="ITypeDiscoveryService"/> to be used. This is only relevant
    /// if <see cref="Mode"/> is set to <see cref="TypeDiscoveryMode.CustomTypeDiscoveryService"/>. In this mode, an 
    /// instance of the specified <see cref="CustomTypeDiscoveryService"/> type is
    /// employed for type discovery. The given type must have a default constructor.
    /// </summary>
    /// <value>A <see cref="TypeElement{TBase}"/> describing the custom <see cref="ITypeDiscoveryService"/> type to be used.</value>
    [ConfigurationProperty ("customTypeDiscoveryService", IsRequired = false)]
    public TypeElement<ITypeDiscoveryService> CustomTypeDiscoveryService
    {
      get { return (TypeElement<ITypeDiscoveryService>) this["customTypeDiscoveryService"]; }
    }

    /// <summary>
    /// Creates an <see cref="ITypeDiscoveryService"/> instance as indicated by <see cref="Mode"/>.
    /// </summary>
    /// <returns>A new <see cref="ITypeDiscoveryService"/> that discovers types as indicated by <see cref="Mode"/>.</returns>
    public ITypeDiscoveryService CreateTypeDiscoveryService ()
    {
      switch (Mode)
      {
        case TypeDiscoveryMode.CustomRootAssemblyFinder:
          return CreateServiceWithCustomRootAssemblyFinder ();
        case TypeDiscoveryMode.SpecificRootAssemblies:
          return CreateServiceWithSpecificRootAssemblies ();
        case TypeDiscoveryMode.CustomTypeDiscoveryService:
          return CreateCustomService ();
        default:
          return CreateServiceWithAutomaticDiscovery ();
      }
    }

    private ITypeDiscoveryService CreateServiceWithCustomRootAssemblyFinder ()
    {
      if (CustomRootAssemblyFinder.Type == null)
      {
        string message = string.Format (
            "In CustomRootAssemblyFinder mode, a custom root assembly finder must be specified in the type discovery configuration. {0}", 
            GetConfigurationBodyErrorMessage (
                "CustomRootAssemblyFinder", 
                "<customRootAssemblyFinder type=\"ApplicationNamespace.CustomFinderType, ApplicationAssembly\"/>"));
        throw new ConfigurationErrorsException (message);
      }

      var customRootAssemblyFinder = (IRootAssemblyFinder) Activator.CreateInstance (CustomRootAssemblyFinder.Type);
      return CreateServiceWithAssemblyFinder (customRootAssemblyFinder);
    }

    private ITypeDiscoveryService CreateServiceWithSpecificRootAssemblies ()
    {
      var rootAssemblyFinder = SpecificRootAssemblies.CreateRootAssemblyFinder ();
      return CreateServiceWithAssemblyFinder (rootAssemblyFinder);
    }

    private ITypeDiscoveryService CreateCustomService ()
    {
      if (CustomTypeDiscoveryService.Type == null)
      {
        string message = string.Format (
            "In CustomTypeDiscoveryService mode, a custom type discovery service must be specified in the type discovery configuration. {0}",
            GetConfigurationBodyErrorMessage (
                "CustomTypeDiscoveryService",
                "<customTypeDiscoveryService type=\"ApplicationNamespace.CustomServiceType, ApplicationAssembly\"/>"));
        throw new ConfigurationErrorsException (message);
      }

      return (ITypeDiscoveryService) Activator.CreateInstance (CustomTypeDiscoveryService.Type);
    }

    private ITypeDiscoveryService CreateServiceWithAutomaticDiscovery ()
    {
      var searchPathRootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (false);
      return CreateServiceWithAssemblyFinder (searchPathRootAssemblyFinder);
    }

    private ITypeDiscoveryService CreateServiceWithAssemblyFinder (IRootAssemblyFinder customRootAssemblyFinder)
    {
      var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new AssemblyFinder (customRootAssemblyFinder, assemblyLoader);
      return new AssemblyFinderTypeDiscoveryService (assemblyFinder);
    }
    
    private string GetConfigurationBodyErrorMessage (string modeValue, string modeSpecificBodyElement)
    {
      var message = Environment.NewLine + Environment.NewLine
                    + "Example configuration: " + Environment.NewLine
                    + "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine
                    + "<configuration>" + Environment.NewLine
                    + "  <configSections>" + Environment.NewLine
                    + "    <section name=\"remotion.typeDiscovery\" type=\"Remotion.Configuration.TypeDiscovery.TypeDiscoveryConfiguration, Remotion\" />" + Environment.NewLine
                    + "  </configSections>" + Environment.NewLine
                    + "  <remotion.typeDiscovery xmlns=\"http://www.re-motion.org/typeDiscovery/configuration\" mode=\"" + modeValue + "\">" + Environment.NewLine
                    + "    " + modeSpecificBodyElement + Environment.NewLine
                    + "  </remotion.typeDiscovery>" + Environment.NewLine
                    + "</configuration>";
      return message;
    }
  }
}
