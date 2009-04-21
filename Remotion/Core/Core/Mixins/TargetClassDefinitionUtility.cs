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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides common functionality for retrieving <see cref="TargetClassDefinition"/> metadata objects.
  /// </summary>
  public class TargetClassDefinitionUtility
  {
    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// this type.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which an analyzed mixin configuration should be returned or a concrete mixed type.</param>
    /// <returns>A non-null <see cref="TargetClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="TargetClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Use <see cref="GetActiveConfiguration(Type,GenerationPolicy)"/> to force generation of an empty configuration if none currently
    /// exists for the given type in <see cref="MixinConfiguration.ActiveConfiguration"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, no new <see cref="TargetClassDefinition"/> is created for it.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetOrConcreteType)
    {
      return GetActiveConfiguration (targetOrConcreteType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which an analyzed mixin configuration should be returned or a concrete mixed type.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetOrConcreteType"/>.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="TargetClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Use the <paramref name="generationPolicy"/> parameter to configure whether this method should return an empty but valid
    /// <see cref="TargetClassDefinition"/> for types that do not have a mixin configuration in <see cref="MixinConfiguration.ActiveConfiguration"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, no new <see cref="TargetClassDefinition"/> is created for it unless
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetOrConcreteType, GenerationPolicy generationPolicy)
    {
      return GetConfiguration (targetOrConcreteType, MixinConfiguration.ActiveConfiguration, generationPolicy);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// this type.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which an analyzed mixin configuration should be returned or a concrete mixed type.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// the given type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="TargetClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Use <see cref="GetConfiguration(Type,MixinConfiguration,GenerationPolicy)"/> to force generation of an empty configuration if none currently
    /// exists for the given type.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, no new <see cref="TargetClassDefinition"/> is created for it.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetOrConcreteType, MixinConfiguration mixinConfiguration)
    {
      return GetConfiguration (targetOrConcreteType, mixinConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which an analyzed mixin configuration should be returned or a concrete mixed type.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetOrConcreteType"/>.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetOrConcreteType"/> violates at least one validation
    /// rule, which would make code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// Use this to retrieve a cached analyzed mixin configuration object for the given target type. The cache is actually maintained by
    /// <see cref="TargetClassDefinitionCache"/>, but this is the public API that should be used instead of directly accessing the cache.
    /// </para>
    /// <para>
    /// Use the <paramref name="generationPolicy"/> parameter to configure whether this method should return an empty but valid
    /// <see cref="TargetClassDefinition"/> for types that do not have a mixin configuration in <paramref name="mixinConfiguration"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, no new <see cref="TargetClassDefinition"/> is created for it unless
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetOrConcreteType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context = GetContext (targetOrConcreteType, mixinConfiguration, generationPolicy);

      if (context == null)
        return null;
      else
        return TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
    }

    /// <summary>
    /// Returns a <see cref="ClassContext"/> for the a given target type.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which a context should be returned or a concrete mixed type.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetOrConcreteType"/>.</param>
    /// <returns>A <see cref="ClassContext"/> for the a given target type, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// Use this to extract a class context for a given target type from an <see cref="MixinConfiguration"/> as it would be used to create the
    /// <see cref="TargetClassDefinition"/> object for the target type. Besides looking up the target type in the given mixin configuration, this
    /// includes generating a default context if <see cref="GenerationPolicy.ForceGeneration"/> is specified and the specialization of generic
    /// arguments in the class context, if any.
    /// </para>
    /// <para>
    /// Use the <paramref name="generationPolicy"/> parameter to configure whether this method should return an empty but valid
    /// <see cref="ClassContext"/> for types that do not have a mixin configuration in the <paramref name="mixinConfiguration"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, the <see cref="ClassContext"/> used for its generation is returned unless
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static ClassContext GetContext (Type targetOrConcreteType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context;
      if (generationPolicy != GenerationPolicy.ForceGeneration && MixinTypeUtility.IsGeneratedConcreteMixedType (targetOrConcreteType))
        context = MixinReflector.GetClassContextFromConcreteType (targetOrConcreteType);
      else
        context = mixinConfiguration.ClassContexts.GetWithInheritance (targetOrConcreteType);

      if (context == null && generationPolicy == GenerationPolicy.ForceGeneration)
        context = new ClassContext (targetOrConcreteType);

      if (context != null && targetOrConcreteType.IsGenericType && context.Type.IsGenericTypeDefinition)
        context = context.SpecializeWithTypeArguments (targetOrConcreteType.GetGenericArguments ());

      return context;
    }
  }
}
