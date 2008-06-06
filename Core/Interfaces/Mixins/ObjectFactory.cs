/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/> with a public constructor.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{TResult}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <typeparamref name="T"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
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
    /// The <see cref="Create{T}(object[])"/> method supports the creation of instances from their complete interfaces:
    /// <typeparamref name="T"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> Create<T> (params object[] preparedMixins)
    {
      return Create<T> (false, preparedMixins);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/> with a public constructor.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
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
    /// The <see cref="Create{T}(GenerationPolicy, object[])"/> method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface registered in the current mixin configuration. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> Create<T> (GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return Create<T> (false, generationPolicy, preparedMixins);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="targetType"/> with a public constructor.
    /// </summary>
    /// <param name="targetType">The target type a mixed instance of which should be created.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="targetType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetType"/> is an interface and it cannot be determined which class
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
    /// The <see cref="Create(Type, object[])"/> method supports the creation of instances from their complete interfaces:
    /// <paramref name="targetType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetType"/> is already a generated type, this method will not subclass it again.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> Create (Type targetType, params object[] preparedMixins)
    {
      return Create (false, targetType, preparedMixins);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="targetType"/> with a public constructor.
    /// </summary>
    /// <param name="targetType">The target type a mixed instance of which should be created.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="targetType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetType"/> is an interface and it cannot be determined which class
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
    /// The <see cref="Create (Type, GenerationPolicy, object[])"/> method supports the creation of instances from their complete
    /// interfaces: <paramref name="targetType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetType"/> is already a generated type, this method will only subclass it again when
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> Create (Type targetType, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return Create (false, targetType, generationPolicy, preparedMixins);
    }

    #endregion

    #region Nonpublic construction
    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
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
    /// The <see cref="Create{T}(object[])"/> method supports the creation of instances from their complete interfaces:
    /// <typeparamref name="T"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> Create<T> (bool allowNonPublicConstructors, params object[] preparedMixins)
    {
      return Create<T> (allowNonPublicConstructors, GenerationPolicy.GenerateOnlyIfConfigured, preparedMixins);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given base type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type a mixed instance of which should be created.</typeparam>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <typeparamref name="T"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <typeparamref name="T"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
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
    /// The <see cref="Create{T}(GenerationPolicy, object[])"/> method supports the creation of instances from their complete interfaces: <typeparamref name="T"/> can be an
    /// interface registered in the current mixin configuration. See also <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<T> Create<T> (bool allowNonPublicConstructors, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return VersionDependentImplementationBridge<IMixedObjectInstantiator>.Implementation
          .CreateConstructorInvoker<T> (typeof (T), generationPolicy, allowNonPublicConstructors, preparedMixins);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="targetType"/>.
    /// </summary>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="targetType">The target type a mixed instance of which should be created.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="targetType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetType"/> is an interface and it cannot be determined which class
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
    /// The <see cref="Create(Type, object[])"/> method supports the creation of instances from their complete interfaces:
    /// <paramref name="targetType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetType"/> is already a generated type, this method will not subclass it again.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> Create (bool allowNonPublicConstructors, Type targetType, params object[] preparedMixins)
    {
      return Create (allowNonPublicConstructors, targetType, GenerationPolicy.GenerateOnlyIfConfigured, preparedMixins);
    }

    /// <summary>
    /// Prepares a creator for a mixed instance of the given <paramref name="targetType"/>.
    /// </summary>
    /// <param name="allowNonPublicConstructors">If true, the factory will also construct objects without a public constructor. If false, an exception is thrown
    /// unless a public constructor is available.</param>
    /// <param name="targetType">The target type a mixed instance of which should be created.</param>
    /// <param name="generationPolicy">Indicates whether a derived class should be generated even for types that do not have an active mixin configuration.</param>
    /// <param name="preparedMixins">The pre-instantiated mixin instances to integrate into the mixed instance. You can specify all, none, or a subset
    /// of the mixins currently configured with <paramref name="targetType"/>. Those mixins for which no
    /// prepared instances are given will be automatically created when the mixed object is constructed.</param>
    /// <returns>An object which can be used to instantiate a mixed type derived from <paramref name="targetType"/> and initialize the instance. Use the object's
    /// <see cref="IFuncInvoker{T}.With()"/> methods to actually create the mixed instance.</returns>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The <paramref name="targetType"/> is an interface and it cannot be determined which class
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
    /// The <see cref="Create (Type, GenerationPolicy, object[])"/> method supports the creation of instances from their complete
    /// interfaces: <paramref name="targetType"/> can be an interface registered in the current mixin configuration. See also
    /// <see cref="CompleteInterfaceAttribute"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetType"/> is already a generated type, this method will only subclass it again when
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static FuncInvokerWrapper<object> Create (bool allowNonPublicConstructors, Type targetType, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      return VersionDependentImplementationBridge<IMixedObjectInstantiator>.Implementation
          .CreateConstructorInvoker<object> (targetType, generationPolicy, allowNonPublicConstructors, preparedMixins);
    }
    #endregion
  }
}
