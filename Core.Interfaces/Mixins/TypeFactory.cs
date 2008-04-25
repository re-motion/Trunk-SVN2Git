using System;
using System.Runtime.Serialization;
using Remotion.Implementation;
using Remotion.Mixins.BridgeInterfaces;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides support for combining mixins and target classes into concrete, "mixed", instantiable types.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a target class should be combined with mixins, the target class (and sometimes also the mixin types) cannot be instantiated as
  /// is. Instead, a concrete type has to be created which incorporates the necessary delegation code. While the type generation is actually performed
  /// by another class, the <see cref="TypeFactory"/> provides the public API to be used when retrieving a generated type.
  /// </para>
  /// <para>
  /// The <see cref="TypeFactory"/> should only be used if <see cref="Type"/> objects are required. If the combined type should be instantiated,
  /// the <see cref="ObjectFactory"/> class should be used instead.
  /// </para>
  /// <para>
  /// The <see cref="TypeFactory"/> class uses the mixin configuration active for the thread on which it is called.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public static class TypeFactory
  {
    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetType"/>, or <paramref name="targetType"/> itself if no
    /// mixin configuration exists for the type on the current thread.
    /// </summary>
    /// <param name="targetType">Base type for which a mixed type should be retrieved.</param>
    /// <returns>A concrete, instantiable, mixed type for the given <paramref name="targetType"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetType"/> violates at least one validation rule, which makes code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// The type returned by this method is guaranteed to be derived from <paramref name="targetType"/>, but will usually not be the same as
    /// <paramref name="targetType"/>. It manages integration of the mixins with the given <paramref name="targetType"/>.
    /// </para>
    /// <para>
    /// Note that the factory will not create derived types for types not currently having a mixin configuration. This means that types returned
    /// by the factory can <b>not</b> always be treated as derived types. See <see cref="GetConcreteType(Type,GenerationPolicy)"/> on how to
    /// force generation of a derived type.
    /// </para>
    /// <para>
    /// The returned type provides the same constructors as <paramref name="targetType"/> does and can thus be instantiated, e.g. via
    /// <see cref="Activator.CreateInstance(Type, object[])"/>. When this happens, all the mixins associated with the generated type are also
    /// instantiated and configured to be used with the target instance. If you need to supply pre-created mixin instances for an object, use
    /// a <em>MixedObjectInstantiationScope</em>. See <see cref="ObjectFactory"/> for a simpler way to immediately create instances of mixed types.
    /// </para>
    /// <para>
    /// If <paramref name="targetType"/> is already a generated type, this method will not subclass it again.
    /// </para>
    /// </remarks>
    public static Type GetConcreteType (Type targetType)
    {
      return GetConcreteType (targetType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">Base type for which a mixed type should be retrieved.</param>
    /// <param name="generationPolicy">Defines whether to force generation of a type even if no mixin configuration is currently available
    /// for the given type.</param>
    /// <returns>A concrete, instantiable, mixed type for the given <paramref name="targetType"/>, or the type itself; depending on the
    /// <paramref name="generationPolicy"/> and the active configuration.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetType"/> violates at least one validation rule, which makes code generation impossible. </exception>
    /// <remarks>
    /// <para>
    /// The type returned by this method is guaranteed to be derived from <paramref name="targetType"/>, but will usually not be the same as
    /// <paramref name="targetType"/>. It manages integration of the mixins with the given <paramref name="targetType"/>.
    /// </para>
    /// <para>
    /// Note that the factory can create empty mixin configurations for types not currently having a mixin configuration, depending on the
    /// <paramref name="generationPolicy"/>. With <see cref="GenerationPolicy.ForceGeneration"/>, types returned by the factory can always be treated
    /// as derived types.
    /// </para>
    /// <para>
    /// The returned type provides the same constructors as <paramref name="targetType"/> does and can thus be instantiated, e.g. via
    /// <see cref="Activator.CreateInstance(Type, object[])"/>. When this happens, all the mixins associated with the generated type are also
    /// instantiated and configured to be used with the target instance. If you need to supply pre-created mixin instances for an object, use
    /// a <em>MixedObjectInstantiationScope</em>. See <see cref="ObjectFactory"/> for a simpler way to immediately create instances of mixed types.
    /// </para>
    /// <para>
    /// If <paramref name="targetType"/> is already a generated type, this method will only subclass it again when
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy)
    {
      return VersionDependentImplementationBridge<ITypeFactoryImplementation>.Implementation.GetConcreteType (targetType, generationPolicy);
    }

    /// <summary>
    /// Initializes a mixin target instance which was created without its constructor having been called.
    /// </summary>
    /// <param name="mixinTarget">The mixin target to initialize.</param>
    /// <exception cref="ArgumentNullException">The mixin target is <see langword="null"/>.</exception>
    /// <remarks>This method is useful when a mixin target instance is created via <see cref="FormatterServices.GetSafeUninitializedObject"/>.</remarks>
    public static void InitializeUnconstructedInstance (object mixinTarget)
    {
      VersionDependentImplementationBridge<ITypeFactoryImplementation>.Implementation.InitializeUnconstructedInstance (mixinTarget);
    }
  }
}