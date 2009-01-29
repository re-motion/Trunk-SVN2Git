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
using Remotion.Implementation;
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Reflection;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides support for instantiating type which are combined with mixins.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a target class should be combined with mixins, the target class (and sometimes also the mixin types) cannot be instantiated as
  /// is. Instead, a concrete type has to be created which incorporates the necessary delegation code, and this type is then instantiated.
  /// The <see cref="ObjectFactory"/> class provides a simple API to creating and instantiating these mixed types. If a mixed type should be
  /// created without being instantiated, refer to the <see cref="TypeFactory"/> class.
  /// </para>
  /// <para>
  /// The <see cref="ObjectFactory"/> class uses the mixin configuration currently active on the current thread. Use the 
  /// <c>MixinConfiguration</c> class if the configuration needs to be adapted.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public static class ObjectFactory
  {
    #region Public construction
    /// <summary>
    /// Creates a mixed instance of the given base type <typeparamref name="T"/> with a public constructor.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <typeparamref name="T"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <typeparamref name="T"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para>
    /// <para>- or -</para><para>The current mixin configuration for 
    /// the <typeparamref name="T"/> violates at least one validation rule, which makes code generation impossible.</para></exception>
    /// <exception cref="Exception"><para>The current mixin configuration for the <typeparamref name="T"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para>
    /// <para>- or -</para><para>The current mixin configuration for 
    /// the <typeparamref name="T"/> violates at least one validation rule, which makes code generation impossible.</para></exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// The <see cref="Create(Type,ParamList,object[])"/> method supports the creation of instances from their complete interfaces:
    /// <typeparamref name="T"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static T Create<T> (ParamList constructorParameters, params object[] preparedMixins)
    {
      return Create<T> (false, constructorParameters, preparedMixins);
    }

    /// <summary>
    /// Creates a mixed instance of the given base type <typeparamref name="T"/> with a public constructor.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <typeparamref name="T"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <typeparamref name="T"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <typeparamref name="T"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface registered in the current mixin configuration. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static T Create<T> (ParamList constructorParameters, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return Create<T> (false, constructorParameters, generationPolicy, preparedMixins);
    }

    /// <summary>
    /// Creates a mixed instance of the given <paramref name="targetOrConcreteType"/> with a public constructor.
    /// </summary>
    /// <param name="targetOrConcreteType">The target type a mixed instance of which should be created or a concrete mixed type.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetOrConcreteType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <paramref name="targetOrConcreteType"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetOrConcreteType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetOrConcreteType"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete interfaces:
    /// <paramref name="targetOrConcreteType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, this method will not subclass it again.
    /// </para>
    /// </remarks>
    public static object Create (Type targetOrConcreteType, ParamList constructorParameters, params object[] preparedMixins)
    {
      return Create (false, targetOrConcreteType, constructorParameters, preparedMixins);
    }

    /// <summary>
    /// Creates a mixed instance of the given <paramref name="targetOrConcreteType"/> with a public constructor.
    /// </summary>
    /// <param name="targetOrConcreteType">The target type a mixed instance of which should be created or a concrete mixed type.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetOrConcreteType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <paramref name="targetOrConcreteType"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetOrConcreteType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetOrConcreteType"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete
    /// interfaces: <paramref name="targetOrConcreteType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, this method will only subclass it again when
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static object Create (Type targetOrConcreteType, ParamList constructorParameters, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return Create (false, targetOrConcreteType, constructorParameters, generationPolicy, preparedMixins);
    }

    #endregion

    #region Nonpublic construction
    /// <summary>
    /// Creates a mixed instance of the given base type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <typeparamref name="T"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <typeparamref name="T"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <typeparamref name="T"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete interfaces:
    /// <typeparamref name="T"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static T Create<T> (bool allowNonPublicConstructors, ParamList constructorParameters, params object[] preparedMixins)
    {
      return Create<T> (allowNonPublicConstructors, constructorParameters, GenerationPolicy.GenerateOnlyIfConfigured, preparedMixins);
    }

    /// <summary>
    /// Creates a mixed instance of the given base type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <typeparamref name="T"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <typeparamref name="T"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <typeparamref name="T"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The base type <typeparamref name="T"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface registered in the current mixin configuration. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static T Create<T> (bool allowNonPublicConstructors, ParamList constructorParameters, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return (T) Create (allowNonPublicConstructors, typeof (T), constructorParameters, generationPolicy, preparedMixins);
    }

    /// <summary>
    /// Creates a mixed instance of the given <paramref name="targetOrConcreteType"/>.
    /// </summary>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="targetOrConcreteType">The target type a mixed instance of which should be created or a concrete mixed type.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetOrConcreteType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <paramref name="targetOrConcreteType"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetOrConcreteType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetOrConcreteType"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/> with
    /// <see cref="GenerationPolicy.GenerateOnlyIfConfigured"/>. This means that mixed types are only created for
    /// instances which do have an active mixin configuration. All other types passed to this method are directly instantiated, without code
    /// generation.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete interfaces:
    /// <paramref name="targetOrConcreteType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, this method will not subclass it again.
    /// </para>
    /// </remarks>
    public static object Create (bool allowNonPublicConstructors, Type targetOrConcreteType, ParamList constructorParameters, params object[] preparedMixins)
    {
      return Create (allowNonPublicConstructors, targetOrConcreteType, constructorParameters, GenerationPolicy.GenerateOnlyIfConfigured, preparedMixins);
    }

    /// <summary>
    /// Creates a mixed instance of the given <paramref name="targetOrConcreteType"/>.
    /// </summary>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="targetOrConcreteType">The target type a mixed instance of which should be created or a concrete mixed type.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetOrConcreteType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>A mixed instance of a type derived from <paramref name="targetOrConcreteType"/>.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetOrConcreteType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetOrConcreteType"/> is an interface and it cannot be determined which class
    /// to instantiate.
    /// </para>
    /// <para>
    /// -or-
    /// </para>
    /// <para>
    /// The <paramref name="preparedMixins"/> parameter contains at least one mixin instance which is not
    /// defined as a mixin for the target type in the current thread's mixin configuration.
    /// </para>
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method internally uses <see cref="TypeFactory.GetConcreteType(Type, GenerationPolicy)"/>. Note that this means that mixed types might
    /// be created even for instances which do not have an active mixin configuration, as specified with the <paramref name="generationPolicy"/>
    /// parameter. In that case, all objects created via this method can be treated in the same way, but it might be inefficient to create arbitrary
    /// non-mixed objects with this policy.
    /// </para>
    /// <para>
    /// This method supports the creation of instances from their complete
    /// interfaces: <paramref name="targetOrConcreteType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, this method will only subclass it again when
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static object Create (bool allowNonPublicConstructors, Type targetOrConcreteType, ParamList constructorParameters, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return VersionDependentImplementationBridge<IMixedObjectInstantiator>.Implementation
          .CreateInstance (allowNonPublicConstructors, targetOrConcreteType, constructorParameters, generationPolicy, preparedMixins);
    }
    #endregion
  }
}
