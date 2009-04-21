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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Assists <see cref="MixinConfigurationBuilder"/> by providing a fluent interface for building <see cref="ClassContext"/> objects.
  /// </summary>
  public class ClassContextBuilder
  {
    private readonly MixinConfigurationBuilder _parent;
    private readonly Type _targetType;
    private readonly Dictionary<Type, MixinContextBuilder> _mixinContextBuilders = new Dictionary<Type, MixinContextBuilder> ();
    private readonly Set<Type> _completeInterfaces = new Set<Type> ();
    private readonly Set<Type> _suppressedMixins = new Set<Type> ();
    private bool _suppressInheritance = false;

    public ClassContextBuilder (Type targetType)
        : this (new MixinConfigurationBuilder (null), targetType, null)
    {
    }

    public ClassContextBuilder (MixinConfigurationBuilder parent, Type targetType, ClassContext parentContext)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      _parent = parent;
      _targetType = targetType;

      if (parentContext != null)
      {
        foreach (MixinContext mixin in parentContext.Mixins)
          AddMixin (mixin.MixinType).WithDependencies (EnumerableUtility.ToArray (mixin.ExplicitDependencies));

        foreach (Type completeInterface in parentContext.CompleteInterfaces)
          AddCompleteInterface (completeInterface);
      }
    }

    /// <summary>
    /// Gets the <see cref="MixinConfigurationBuilder"/> used for creating this <see cref="ClassContextBuilder"/>.
    /// </summary>
    /// <value>This object's <see cref="MixinConfigurationBuilder"/>.</value>
    public MixinConfigurationBuilder Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Gets the type configured by this <see cref="ClassContextBuilder"/>.
    /// </summary>
    /// <value>The target type configured by this object.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets the mixin context builders collected so far.
    /// </summary>
    /// <value>The mixin context builders collected so far by this object.</value>
    public IEnumerable<MixinContextBuilder> MixinContextBuilders
    {
      get { return _mixinContextBuilders.Values; }
    }

    /// <summary>
    /// Gets the complete interfaces collected so far.
    /// </summary>
    /// <value>The complete interfaces collected so far by this object.</value>
    public IEnumerable<Type> CompleteInterfaces
    {
      get { return _completeInterfaces; }
    }

    /// <summary>
    /// Gets the suppressed mixins collected so far.
    /// </summary>
    /// <value>The suppressed mixins collected so far by this object.</value>
    public IEnumerable<Type> SuppressedMixins
    {
      get { return _suppressedMixins; }
    }

    public bool SuppressInheritance
    {
      get { return _suppressInheritance; }
    }

    /// <summary>
    /// Clears all mixin configuration for the <see cref="TargetType"/>. This causes the target type to ignore all mixin configuration data from its
    /// parent context and also resets all information collected so far for the class by this object.
    /// </summary>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder Clear ()
    {
      _mixinContextBuilders.Clear();
      _completeInterfaces.Clear();
      _suppressInheritance = true;
      return this;
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      if (AlreadyAppliedSame (mixinType))
      {
        Type mixinTypeForException = mixinType.IsGenericType ? mixinType.GetGenericTypeDefinition() : mixinType;
        throw new ArgumentException (
            string.Format ("{0} is already configured as a mixin for type {1}.", mixinTypeForException.FullName, TargetType.FullName), "mixinType");
      }

      MixinContextBuilder mixinContextBuilder = new MixinContextBuilder (this, mixinType);
      _mixinContextBuilders.Add (mixinType, mixinContextBuilder);
      return mixinContextBuilder;
    }

    private bool AlreadyAppliedSame (Type mixinType)
    {
      if (_mixinContextBuilders.ContainsKey (mixinType))
        return true;

      if (!mixinType.IsGenericType)
        return false;

      Type typeDefinition = mixinType.GetGenericTypeDefinition();

      foreach (MixinContextBuilder mixinContextBuilder in MixinContextBuilders)
      {
        if (mixinContextBuilder.MixinType.IsGenericType && mixinContextBuilder.MixinType.GetGenericTypeDefinition() == typeDefinition)
          return true;
      }

      return false;
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin<TMixin> ()
    {
      return AddMixin (typeof (TMixin));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        AddMixin (mixinType);
      return this;
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      return AddMixins (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return AddMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual MixinContextBuilder EnsureMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      MixinContextBuilder builder;
      if (!_mixinContextBuilders.TryGetValue (mixinType, out builder))
        builder = AddMixin (mixinType);
      return builder;
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual MixinContextBuilder EnsureMixin<TMixin>()
    {
      return EnsureMixin (typeof (TMixin));
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        EnsureMixin (mixinType);
      return this;
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2> ()
    {
      return EnsureMixins (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return EnsureMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect with dependencies.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      Type lastMixinType = null;
      foreach (Type mixinType in mixinTypes)
      {
        MixinContextBuilder mixinContextBuilder = AddMixin (mixinType);
        if (lastMixinType != null)
          mixinContextBuilder.WithDependency (lastMixinType);
        lastMixinType = mixinType;
      }
      return this;
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The first mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      return AddOrderedMixins (typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin3">The first mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return AddOrderedMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceType">The type to collect as a complete interface.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      if (_completeInterfaces.Contains (interfaceType))
      {
        string message = string.Format ("{0} is already configured as a complete interface for type {1}.",
            interfaceType.FullName, TargetType.FullName);
        throw new ArgumentException (message, "interfaceType");
      }
      _completeInterfaces.Add (interfaceType);
      return this;
    }

    /// <summary>
    /// Adds the given type as a complete interface to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface">The type to collect as a complete interface.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterface<TInterface> ()
    {
      return AddCompleteInterface (typeof (TInterface));
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceTypes">The types to collect as complete interfaces.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces (params Type[] interfaceTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("interfaceTypes", interfaceTypes);
      foreach (Type interfaceType in interfaceTypes)
        AddCompleteInterface (interfaceType);
      return this;
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2> ()
    {
      return AddCompleteInterfaces (typeof (TInterface1), typeof (TInterface2));
    }

    /// <summary>
    /// Adds the given types as complete interfaces to the <see cref="TargetType"/>. A complete interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as complete interfaces.</typeparam>
    /// <typeparam name="TInterface3">The types to collect as complete interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return AddCompleteInterfaces (typeof (TInterface1), typeof (TInterface2), typeof (TInterface3));
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="mixinType">The mixin type, base type, or generic type definition denoting mixin types to be suppressed.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      if (_suppressedMixins.Contains (mixinType))
      {
        string message =
            string.Format ("The mixin type {0} has already been suppressed for target type {1}.", mixinType.FullName, TargetType.FullName);
        throw new ArgumentException (message, "mixinType");
      }

      _suppressedMixins.Add (mixinType);
      return this;
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType">The mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin<TMixinType> ()
    {
      return SuppressMixin (typeof (TMixinType));
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="mixinTypes">The mixin types, base types, or generic type definitions denoting mixin types to be suppressed.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        SuppressMixin (mixinType);
      return this;
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins<TMixinType1, TMixinType2> ()
    {
      return SuppressMixins (typeof (TMixinType1), typeof (TMixinType2));
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="ReflectionUtility.CanAscribe"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType3">The third mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins<TMixinType1, TMixinType2, TMixinType3> ()
    {
      return SuppressMixins (typeof (TMixinType1), typeof (TMixinType2), typeof (TMixinType3));
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="TargetType"/> that inherits from other contexts.
    /// </summary>
    /// <param name="inheritedContexts">A collection of <see cref="ClassContext"/> instances the newly built context should inherit mixin data from.</param>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext (IEnumerable<ClassContext> inheritedContexts)
    {
      ClassContext classContext = new ClassContext (_targetType, GetMixins(), CompleteInterfaces);
      classContext = ApplyInheritance(classContext, inheritedContexts);
      classContext = classContext.SuppressMixins (SuppressedMixins);
      return classContext;
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="TargetType"/> without inheriting from other contexts.
    /// </summary>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext ()
    {
      return BuildClassContext (new ClassContext[0]);
    }

    private IEnumerable<MixinContext> GetMixins ()
    {
      foreach (MixinContextBuilder mixinContextBuilder in MixinContextBuilders)
        yield return mixinContextBuilder.BuildMixinContext();
    }

    private ClassContext ApplyInheritance (ClassContext classContext, IEnumerable<ClassContext> inheritedContexts)
    {
      if (SuppressInheritance)
        return classContext;
      else
        return classContext.InheritFrom (inheritedContexts);
    }

    #region Parent members

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
      return _parent.ForClass<TTargetType>();
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration();
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope();
    }
    #endregion
  }
}
