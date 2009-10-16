// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Configuration;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures the type discovery performed by <see cref="ContextAwareTypeDiscoveryUtility"/>.
  /// </summary>
  public sealed class TypeDiscoveryConfiguration : ConfigurationSection
  {
    public TypeDiscoveryConfiguration ()
    {
      var xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      Properties.Add (xmlnsProperty);
    }

    [ConfigurationProperty ("mode", DefaultValue = TypeDiscoveryMode.Automatic, IsRequired = false)]
    public TypeDiscoveryMode Mode
    {
      get { return (TypeDiscoveryMode) this["mode"]; }
      set { this["mode"] = value; }
    }

    [ConfigurationProperty ("customRootAssemblyFinder", IsRequired = false)]
    public TypeElement<IRootAssemblyFinder> CustomRootAssemblyFinder
    {
      get { return (TypeElement<IRootAssemblyFinder>) this["customRootAssemblyFinder"]; }
    }

    [ConfigurationProperty ("specificRootAssemblies", IsRequired = false)]
    public RootAssembliesElement SpecificRootAssemblies
    {
      get { return (RootAssembliesElement) this["specificRootAssemblies"]; }
    }

    [ConfigurationProperty ("customTypeDiscoveryService", IsRequired = false)]
    public TypeElement<ITypeDiscoveryService> CustomTypeDiscoveryService
    {
      get { return (TypeElement<ITypeDiscoveryService>) this["customTypeDiscoveryService"]; }
    }

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
            "In CustomRootAssemblyFinder mode, a custom root asembly finder must be specified in the type discovery configuration. {0}", 
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