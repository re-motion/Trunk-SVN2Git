using System;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  public class TargetClassDefinitionUtility
  {
    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// this type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <returns>A non-null <see cref="TargetClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
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
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetType)
    {
      return GetActiveConfiguration (targetType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetType"/>.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
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
    /// </remarks>
    public static TargetClassDefinition GetActiveConfiguration (Type targetType, GenerationPolicy generationPolicy)
    {
      return GetConfiguration (targetType, MixinConfiguration.ActiveConfiguration, generationPolicy);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// this type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/> if no mixin configuration exists for
    /// the given type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
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
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetType, MixinConfiguration mixinConfiguration)
    {
      return GetConfiguration (targetType, mixinConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Returns a <see cref="TargetClassDefinition"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which an analyzed mixin configuration should be returned.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetType"/>.</param>
    /// <returns>A <see cref="TargetClassDefinition"/> for the a given target type, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ConfigurationException">The current mixin configuration for the <paramref name="targetType"/> contains severe problems that
    /// make generation of a <see cref="TargetClassDefinition"/> object impossible.</exception>
    /// <exception cref="ValidationException">The current mixin configuration for the <paramref name="targetType"/> violates at least one validation
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
    /// If <paramref name="targetType"/> is already a generated type, no new <see cref="TargetClassDefinition"/> is created for it unless
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static TargetClassDefinition GetConfiguration (Type targetType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context;
      context = GetContext (targetType, mixinConfiguration, generationPolicy);

      if (context == null)
        return null;
      else
        return TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
    }

    /// <summary>
    /// Returns a <see cref="ClassContext"/> for the a given target type.
    /// </summary>
    /// <param name="targetType">Base type for which a context should be returned.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetType"/>.</param>
    /// <returns>A <see cref="ClassContext"/> for the a given target type, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
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
    /// If <paramref name="targetType"/> is already a generated type, the <see cref="ClassContext"/> used for its generation is returned unless
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static ClassContext GetContext (Type targetType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context;
      if (generationPolicy != GenerationPolicy.ForceGeneration && TypeUtility.IsGeneratedConcreteMixedType (targetType))
        context = MixinReflector.GetClassContextFromConcreteType (targetType);
      else
        context = mixinConfiguration.ClassContexts.GetWithInheritance (targetType);

      if (context == null && generationPolicy == GenerationPolicy.ForceGeneration)
        context = new ClassContext (targetType);

      if (context != null && targetType.IsGenericType && context.Type.IsGenericTypeDefinition)
        context = context.SpecializeWithTypeArguments (targetType.GetGenericArguments ());

      return context;
    }
  }
}