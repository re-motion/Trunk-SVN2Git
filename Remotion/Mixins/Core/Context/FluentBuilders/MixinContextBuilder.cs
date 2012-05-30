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
using Remotion.Collections;
using Remotion.Mixins.Context.Suppression;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Assists <see cref="MixinConfigurationBuilder"/> by providing a fluent interface for building <see cref="MixinContext"/> objects.
  /// </summary>
  public class MixinContextBuilder
  {
    private readonly ClassContextBuilder _parent;
    private readonly Type _mixinType;
    private readonly Set<Type> _dependencies = new Set<Type> ();

    private MixinKind _mixinKind;
    private MemberVisibility _introducedMemberVisiblity;

    public MixinContextBuilder (ClassContextBuilder parent, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _parent = parent;
      _mixinType = mixinType;
      _mixinKind = MixinKind.Extending;
      _introducedMemberVisiblity = MemberVisibility.Private;
    }

    /// <summary>
    /// Gets the <see cref="ClassContextBuilder"/> used for creating this <see cref="MixinContextBuilder"/>.
    /// </summary>
    /// <value>This object's <see cref="ClassContextBuilder"/>.</value>
    public ClassContextBuilder Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Gets the kind of relationship the configured mixin has with its target class.
    /// </summary>
    /// <value>The mixin kind.</value>
    public MixinKind MixinKind
    {
      get { return _mixinKind; }
    }

    /// <summary>
    /// Gets mixin type configured by this object.
    /// </summary>
    /// <value>The mixin type configured by this object.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    /// <summary>
    /// Gets the base call dependencies collected so far.
    /// </summary>
    /// <value>The base call dependencies collected so far by this object.</value>
    public IEnumerable<Type> Dependencies
    {
      get { return _dependencies; }
    }

    /// <summary>
    /// Gets the default introduced member visiblity for this mixin.
    /// </summary>
    /// <value>The default introduced member visiblity.</value>
    public MemberVisibility IntroducedMemberVisiblity
    {
      get { return _introducedMemberVisiblity; }
    }

    /// <summary>
    /// Defines the relationship the mixin has with its target class, which influences whether the mixin overrides attributes and interfaces
    /// of the target class, or the other way around. For more information see <see cref="Mixins.MixinKind"/>. The default value
    /// is <see cref="Mixins.MixinKind.Extending"/>.
    /// </summary>
    /// <param name="kind">The mixin kind.</param>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder OfKind (MixinKind kind)
    {
      _mixinKind = kind;
      return this;
    }

    /// <summary>
    /// Collects a dependency for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <param name="requiredMixin">The mixin required by the configured <see cref="MixinType"/>.</param>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder WithDependency (Type requiredMixin)
    {
      ArgumentUtility.CheckNotNull ("requiredMixin", requiredMixin);
      if (_dependencies.Contains (requiredMixin))
      {
        string message = string.Format ("The mixin {0} already has a dependency on type {1}.", MixinType.FullName, requiredMixin.FullName);
        throw new ArgumentException (message, "requiredMixin");
      }
      _dependencies.Add (requiredMixin);
      return this;
    }

    /// <summary>
    /// Collects a dependency for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <typeparam name="TRequiredMixin">The mixin (or an interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder WithDependency<TRequiredMixin> ()
    {
      return WithDependency (typeof (TRequiredMixin));
    }

    /// <summary>
    /// Collects a number of dependencies for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between
    /// two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <param name="requiredMixins">The mixins (or interfaces) required by the configured <see cref="MixinType"/>.</param>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder WithDependencies (params Type[] requiredMixins)
    {
      ArgumentUtility.CheckNotNull ("requiredMixins", requiredMixins);
      foreach (Type requiredMixin in requiredMixins)
        WithDependency (requiredMixin);
      return this;
    }

    /// <summary>
    /// Collects a number of dependencies for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between
    /// two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <typeparam name="TMixin2">The second mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder WithDependencies<TMixin1, TMixin2> ()
    {
      return WithDependencies (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects a number of dependencies for the configured <see cref="MixinType"/>. A dependency causes a base call ordering to be defined between
    /// two mixins:
    /// if mixin A depends on mixin B and both override the same methods, A's overrides will be called before B's overrides when an overridden member
    /// is invoked.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <typeparam name="TMixin2">The second mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <typeparam name="TMixin3">The third mixin (or interface) required by the configured <see cref="MixinType"/>.</typeparam>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder WithDependencies<TMixin1, TMixin2, TMixin3> ()
    {
      return WithDependencies (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Sets the default visibility of members introduced by this mixin to the given <paramref name="memberVisibility"/>.
    /// </summary>
    /// <param name="memberVisibility">The default member visibility to be used.</param>
    /// <returns>This object for further configuration of the mixin.</returns>
    public MixinContextBuilder WithIntroducedMemberVisibility (MemberVisibility memberVisibility)
    {
      _introducedMemberVisiblity = memberVisibility;
      return this;
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// replaced by this mixin type.
    /// </summary>
    /// <param name="replacedMixinType">The mixin type, base type, or generic type definition denoting mixin types to be replaced.</param>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder ReplaceMixin (Type replacedMixinType)
    {
      ArgumentUtility.CheckNotNull ("replacedMixinType", replacedMixinType);
      Assertion.IsNotNull (_mixinType);

      if (replacedMixinType == _mixinType || (_mixinType.IsGenericType && replacedMixinType == _mixinType.GetGenericTypeDefinition ()))
      {
        string message = string.Format ("Mixin type '{0}' applied to target class '{1}' suppresses itself.", _mixinType, _parent.TargetType);
        throw new InvalidOperationException (message);
      }

      _parent.SuppressMixin (new MixinTreeReplacementSuppressionRule (MixinType, replacedMixinType));
      return this;
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// replaced by this mixin type.
    /// </summary>
    /// <typeparam name="TReplacedMixinType">The mixin type, base type, or generic type definition denoting mixin types to be replaced.</typeparam>
    /// <returns>This object for further configuration of the mixin.</returns>
    public MixinContextBuilder ReplaceMixin<TReplacedMixinType> ()
    {
      return ReplaceMixin (typeof (TReplacedMixinType));
    }

    /// <summary>
    /// Denotes that specific mixin types, and all mixin types that can be ascribed to them (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// replaced by this mixin type.
    /// </summary>
    /// <param name="replacedMixinTypes">The mixin types, base types, or generic type definitions denoting mixin types to be replaced.</param>
    /// <returns>This object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder ReplaceMixins (params Type[] replacedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("replacedMixinTypes", replacedMixinTypes);
      foreach (var replacedMixinType in replacedMixinTypes)
      {
        ReplaceMixin (replacedMixinType);
      }
      return this;
    }

    /// <summary>
    /// Denotes that specific mixin types, and all mixin types that can be ascribed to them (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// replaced by this mixin type.
    /// </summary>
    /// <typeparam name="TReplacedMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be replaced.</typeparam>
    /// <typeparam name="TReplacedMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be replaced.</typeparam>
    /// <returns>This object for further configuration of the mixin.</returns>
    public MixinContextBuilder ReplaceMixins<TReplacedMixinType1, TReplacedMixinType2>  ()
    {
      return ReplaceMixins (typeof (TReplacedMixinType1), typeof (TReplacedMixinType2));
    }

    /// <summary>
    /// Denotes that specific mixin types, and all mixin types that can be ascribed to them (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// replaced by this mixin type.
    /// </summary>
    /// <typeparam name="TReplacedMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be replaced.</typeparam>
    /// <typeparam name="TReplacedMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be replaced.</typeparam>
    /// <typeparam name="TReplacedMixinType3">The third mixin type, base type, or generic type definition denoting mixin types to be replaced.</typeparam>
    /// <returns>This object for further configuration of the mixin.</returns>
    public MixinContextBuilder ReplaceMixins<TReplacedMixinType1, TReplacedMixinType2, TReplacedMixinType3> ()
    {
      return ReplaceMixins (typeof (TReplacedMixinType1), typeof (TReplacedMixinType2), typeof (TReplacedMixinType3));
    }

    /// <summary>
    /// Builds a mixin context with the data collected so far for the <see cref="MixinType"/>.
    /// </summary>
    /// <returns>A <see cref="MixinContext"/> holding all mixin configuration data collected so far.</returns>
    public virtual MixinContext BuildMixinContext ()
    {
      // TODO 1554: TODO
      var origin = new MixinContextOrigin ("Imperative configuration API", typeof (MixinContextBuilder).Assembly, "unknown");
      return new MixinContext (_mixinKind, _mixinType, _introducedMemberVisiblity, _dependencies, origin);
    }

    #region Parent members

    /// <summary>
    /// Clears all mixin configuration for the <see cref="Parent"/>'s <see cref="ClassContextBuilder.TargetType"/>. This causes the target type to ignore
    /// all mixin configuration data from its
    /// parent context and also resets all information collected so far for the class by this object.
    /// </summary>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder Clear ()
    {
      return _parent.Clear();
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin (Type mixinType)
    {
      return _parent.AddMixin (mixinType);
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin<TMixin> ()
    {
      return _parent.AddMixin<TMixin>();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      return _parent.AddMixins (mixinTypes);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      return _parent.AddMixins<TMixin1, TMixin2> ();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.AddMixins<TMixin1, TMixin2, TMixin3>();
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="ClassContextBuilder.TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it is also present in the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual MixinContextBuilder EnsureMixin (Type mixinType)
    {
      return _parent.EnsureMixin (mixinType);
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="ClassContextBuilder.TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it is also present in the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual MixinContextBuilder EnsureMixin<TMixin> ()
    {
      return _parent.EnsureMixin<TMixin>();
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="ClassContextBuilder.TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins (params Type[] mixinTypes)
    {
      return _parent.EnsureMixins (mixinTypes);
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="ClassContextBuilder.TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2> ()
    {
      return _parent.EnsureMixins<TMixin1, TMixin2>();
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="ClassContextBuilder.TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they are also present in the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.EnsureMixins<TMixin1, TMixin2, TMixin3>();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect with dependencies.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      return _parent.AddOrderedMixins (mixinTypes);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect with dependencies.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      return _parent.AddOrderedMixins<TMixin1, TMixin2> ();
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="ClassContextBuilder.TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect with dependencies.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.AddOrderedMixins<TMixin1, TMixin2, TMixin3> ();
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceType">The type to collect as a complete interface.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface (Type interfaceType)
    {
      return _parent.AddCompleteInterface (interfaceType);
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface">The type to collect as a complete interface.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface<TInterface> ()
    {
      return _parent.AddCompleteInterface<TInterface> ();
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceTypes">The types to collect as complete interfaces.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces (params Type[] interfaceTypes)
    {
      return _parent.AddCompleteInterfaces (interfaceTypes);
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2> ()
    {
      return _parent.AddCompleteInterfaces<TInterface1, TInterface2> ();
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="ClassContextBuilder.TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface3">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return _parent.AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ();
    }

    /// <summary>
    /// Denotes that specific mixin types should be ignored in the context of this class. Suppression is helpful when a target class should take 
    /// over most of its mixins from the parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="rule">A <see cref="IMixinSuppressionRule"/> denoting mixin types to be suppressed.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin (IMixinSuppressionRule rule)
    {
      return _parent.SuppressMixin (rule);
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this object's <see cref="ClassContextBuilder"/>. Suppression is helpful when a target class should take over most 
    /// of its mixins from the parent context or inherit mixins from another type, but a specific mixin should be ignored in that 
    /// process.
    /// </summary>
    /// <param name="mixinType">The mixin type, base type, or generic type definition denoting mixin types to be suppressed.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin (Type mixinType)
    {
      return _parent.SuppressMixin (mixinType);
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this object's <see cref="ClassContextBuilder"/>. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType">The mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin<TMixinType> ()
    {
      return _parent.SuppressMixin<TMixinType>();
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this object's <see cref="ClassContextBuilder"/>. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="mixinTypes">The mixin types, base types, or generic type definitions denoting mixin types to be suppressed.</param>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins (params Type[] mixinTypes)
    {
      return _parent.SuppressMixins (mixinTypes);
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this object's <see cref="ClassContextBuilder"/>. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins<TMixinType1, TMixinType2> ()
    {
      return _parent.SuppressMixins<TMixinType1, TMixinType2>();
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this object's <see cref="ClassContextBuilder"/>. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType3">The third mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object's <see cref="ClassContextBuilder"/> for further configuration of the <see cref="ClassContextBuilder.TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins<TMixinType1, TMixinType2, TMixinType3> ()
    {
      return _parent.SuppressMixins<TMixinType1, TMixinType2, TMixinType3>();
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="ClassContextBuilder.TargetType"/> that inherits from other contexts.
    /// </summary>
    /// <param name="inheritedContexts">A collection of <see cref="ClassContext"/> instances the newly built context should inherit mixin data from.</param>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="ClassContextBuilder.TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext (IEnumerable<ClassContext> inheritedContexts)
    {
      return _parent.BuildClassContext (inheritedContexts);
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="ClassContextBuilder.TargetType"/> without inheriting from other contexts.
    /// </summary>
    public virtual ClassContext BuildClassContext ()
    {
      return _parent.BuildClassContext ();
    }

    /// <summary>
    /// Begins configuration of another target class.
    /// </summary>
    /// <param name="targetType">The class to be configured.</param>
    /// <returns>A fluent interface object for configuring the given <paramref name="targetType"/>.</returns>
    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      return _parent.ForClass (targetType);
    }

    /// <summary>
    /// Begins configuration of another target class.
    /// </summary>
    /// <typeparam name="TTargetType">The class to be configured.</typeparam>
    /// <returns>A fluent interface object for configuring the given <typeparamref name="TTargetType"/>.</returns>
    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return _parent.ForClass<TTargetType> ();
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration ();
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope ();
    }
    #endregion
  }
}
