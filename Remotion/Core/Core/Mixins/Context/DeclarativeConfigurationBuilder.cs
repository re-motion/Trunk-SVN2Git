// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Remotion.Collections;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Provides support for building mixin configuration data from the declarative mixin configuration attributes
  /// (<see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, <see cref="CompleteInterfaceAttribute"/>,
  /// and <see cref="IgnoreForMixinConfigurationAttribute"/>).
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public class DeclarativeConfigurationBuilder
  {
    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given assemblies.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="assemblies">The assemblies to be scanned for declarative mixin information.</param>
    /// <returns>An mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the configuration information
    /// held by the given assemblies.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromAssemblies (MixinConfiguration parentConfiguration, IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildDerivedConfiguration (parentConfiguration, delegate (DeclarativeConfigurationBuilder builder)
      {
        foreach (Assembly assembly in assemblies)
          builder.AddAssembly (assembly);
      });
    }

    private static MixinConfiguration BuildDerivedConfiguration (MixinConfiguration parentConfiguration, System.Action<DeclarativeConfigurationBuilder> overrideGenerator)
    {
      ArgumentUtility.CheckNotNull ("overrideGenerator", overrideGenerator);
      
      // DeclarativeConfigurationBuilder will not overwrite any existing class contexts, but instead augment them with any new definitions.
      // This is exactly what we want for the generated overrides, since all of these are of equal priority. However, we want to replace any
      // conflicting contexts inherited from the parent configuration.

      // Therefore, we first analyze all overrides into a temporary configuration without replacements:
      DeclarativeConfigurationBuilder tempConfigurationBuilder = new DeclarativeConfigurationBuilder (null);
      overrideGenerator (tempConfigurationBuilder);

      MixinConfiguration tempConfiguration = tempConfigurationBuilder.BuildConfiguration ();

      // Then, we add the analyzed data to the result context, replacing the respective inherited class contexts:
      MixinConfiguration fullConfiguration = new MixinConfiguration (parentConfiguration);
      tempConfiguration.CopyTo (fullConfiguration);
      return fullConfiguration;
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given assemblies.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="assemblies">The assemblies to be scanned for declarative mixin information.</param>
    /// <returns>A mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the configuration information
    /// held by the given assemblies.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromAssemblies (MixinConfiguration parentConfiguration, params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildConfigurationFromAssemblies (parentConfiguration, (IEnumerable<Assembly>) assemblies);
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given assemblies without inheriting
    /// from a parent configuration.
    /// </summary>
    /// <param name="assemblies">The assemblies to be scanned for declarative mixin information.</param>
    /// <returns>A mixin configuration incorporating the configuration information held by the given assemblies.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromAssemblies (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return BuildConfigurationFromAssemblies (null, (IEnumerable<Assembly>) assemblies);
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> containing the given class contexts (replacing conflicting inherited ones, if any).
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="classContexts">The class contexts to be contained in the new mixin configuration.</param>
    /// <returns>A mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the given class contexts.</returns>
    public static MixinConfiguration BuildConfigurationFromClasses (MixinConfiguration parentConfiguration, params ClassContext[] classContexts)
    {
      ArgumentUtility.CheckNotNull ("classContexts", classContexts);

      MixinConfiguration configuration = new MixinConfiguration (parentConfiguration);

      foreach (ClassContext classContext in classContexts)
        configuration.ClassContexts.AddOrReplace (classContext);

      return configuration;
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given types.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="types">The types to be scanned for declarative mixin information.</param>
    /// <returns>A mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the configuration information
    /// held by the given types.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="types"/> parameter is <see langword="null"/>.</exception>
    private static MixinConfiguration BuildConfigurationFromTypes (MixinConfiguration parentConfiguration, IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return BuildDerivedConfiguration (parentConfiguration, delegate (DeclarativeConfigurationBuilder builder)
      {
        foreach (Type type in types)
        {
          if (!type.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
            builder.AddType (type);
        }
      });
    }

    /// <summary>
    /// Builds the default application configuration by analyzing all assemblies in the application bin directory and their (directly or indirectly)
    /// referenced assemblies for mixin configuration information. System assemblies are not scanned.
    /// </summary>
    /// <returns>A mixin configuration holding the default mixin configuration information for this application.</returns>
    /// <remarks>This method uses the <see cref="ContextAwareTypeDiscoveryUtility"/> to discover the types to be used in the mixin configuration.
    /// In design mode, this will use the types returned by the designer, but in ordinary application scenarios, the following steps are performed:
    /// <list type="number">
    /// <item>Retrieve all types assemblies from the current <see cref="AppDomain">AppDomain's</see> bin directory.</item>
    /// <item>Analyze each of them that is included by the <see cref="ApplicationAssemblyFinderFilter"/> for mixin configuration information.</item>
    /// <item>Load the referenced assemblies of those assemblies if they aren't excluded by the <see cref="ApplicationAssemblyFinderFilter"/>.</item>
    /// <item>If the loaded assemblies haven't already been analyzed, treat them according to steps 2-4.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ContextAwareTypeDiscoveryUtility"/>
    public static MixinConfiguration BuildDefaultConfiguration ()
    {
      ICollection types = GetTypeDiscoveryService().GetTypes (null, false);
      return BuildConfigurationFromTypes (null, EnumerableUtility.Cast<Type> (types));
    }

    private static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      return ContextAwareTypeDiscoveryUtility.GetInstance();
    }

    private readonly MixinConfiguration _parentConfiguration;
    private readonly Set<Type> _allTypes = new Set<Type> ();

    /// <summary>
    /// Initializes a new <see cref="DeclarativeConfigurationBuilder"/>, which can be used to collect assemblies and types with declarative
    /// mixin configuration attributes in order to build an <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration used when this instance builds a new <see cref="MixinConfiguration"/>.</param>
    public DeclarativeConfigurationBuilder (MixinConfiguration parentConfiguration)
    {
      _parentConfiguration = parentConfiguration;
    }

    public IEnumerable<Type> AllTypes
    {
      get { return _allTypes; }
    }

    /// <summary>
    /// Scans the given assembly for declarative mixin configuration information and stores the information for a later call to <see cref="BuildConfiguration"/>.
    /// The mixin configuration information of types marked with the <see cref="IgnoreForMixinConfigurationAttribute"/> will be ignored.
    /// </summary>
    /// <param name="assembly">The assembly to be scanned.</param>
    /// <returns>A reference to this <see cref="DeclarativeConfigurationBuilder"/> object.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
    public DeclarativeConfigurationBuilder AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      foreach (Type t in assembly.GetTypes())
      {
        if (!t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false) && !MixinTypeUtility.IsGeneratedByMixinEngine (t))
          AddType (t);
      }
      return this;
    }

    /// <summary>
    /// Scans the given type for declarative mixin configuration information and stores the information for a later call to <see cref="BuildConfiguration"/>.
    /// The type will be scanned whether or not is is marked with the <see cref="IgnoreForMixinConfigurationAttribute"/>.
    /// </summary>
    /// <param name="type">The type to be scanned. This must be a non-generic type or a generic type definition. Closed generic types are not
    /// supported to be scanned.</param>
    /// <returns>A reference to this <see cref="DeclarativeConfigurationBuilder"/> object.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given type is a closed generic type and not a generic type definition.</exception>
    public DeclarativeConfigurationBuilder AddType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        throw new ArgumentException ("Type must be non-generic or a generic type definition.", "type");

      _allTypes.Add (type);

      if (type.BaseType != null)
      {
        // When analyzing types for attributes, we want type definitions, not specializations
        if (type.BaseType.IsGenericType)
          AddType (type.BaseType.GetGenericTypeDefinition());
        else
          AddType (type.BaseType);
      }

      return this;
    }

    /// <summary>
    /// Analyzes the information added so far to this builder and creates a new <see cref="MixinConfiguration"/> from that data.
    /// </summary>
    /// <returns>An <see cref="MixinConfiguration"/> derived from the configuration specified in the builder's constructor containing
    /// <see cref="ClassContext"/> and <see cref="MixinContext"/> objects based on the information added so far.</returns>
    public MixinConfiguration BuildConfiguration ()
    {
      MixinConfigurationBuilder configurationBuilder = new MixinConfigurationBuilder (_parentConfiguration);
      ExtendsAnalyzer extendsAnalyzer = new ExtendsAnalyzer (configurationBuilder);
      UsesAnalyzer usesAnalyzer = new UsesAnalyzer (configurationBuilder);
      CompleteInterfaceAnalyzer completeInterfaceAnalyzer = new CompleteInterfaceAnalyzer (configurationBuilder);
      MixAnalyzer mixAnalyzer = new MixAnalyzer (configurationBuilder);
      IgnoresAnalyzer ignoresAnalyzer = new IgnoresAnalyzer (configurationBuilder);
      
      DeclarativeConfigurationAnalyzer configurationAnalyzer =
          new DeclarativeConfigurationAnalyzer (extendsAnalyzer, usesAnalyzer, completeInterfaceAnalyzer, mixAnalyzer, ignoresAnalyzer);
      configurationAnalyzer.Analyze(_allTypes);
      
      return configurationBuilder.BuildConfiguration();
    }
  }
}
