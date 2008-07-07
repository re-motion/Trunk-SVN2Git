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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Provides a fluent interface for building <see cref="MixinConfiguration"/> objects.
  /// </summary>
  public class MixinConfigurationBuilder
  {
    private readonly MixinConfiguration _parentConfiguration;
    private readonly Dictionary<Type, ClassContextBuilder> _classContextBuilders = new Dictionary<Type, ClassContextBuilder>();

    public MixinConfigurationBuilder (MixinConfiguration parentConfiguration)
    {
      _parentConfiguration = parentConfiguration;
    }

    /// <summary>
    /// Gets the parent configuration used as a base for the newly built mixin configuration.
    /// </summary>
    /// <value>The parent configuration.</value>
    public virtual MixinConfiguration ParentConfiguration
    {
      get { return _parentConfiguration; }
    }

    /// <summary>
    /// Gets the class context builders collected so far via the fluent interfaces.
    /// </summary>
    /// <value>The class context builders collected so far.</value>
    public IEnumerable<ClassContextBuilder> ClassContextBuilders
    {
      get { return _classContextBuilders.Values; }
    }

    /// <summary>
    /// Begins configuration of a target class.
    /// </summary>
    /// <param name="targetType">The class to be configured.</param>
    /// <returns>A fluent interface object for configuring the given <paramref name="targetType"/>.</returns>
    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      if (!_classContextBuilders.ContainsKey (targetType))
      {
        ClassContext parentContext = ParentConfiguration != null ? ParentConfiguration.ClassContexts.GetExact (targetType) : null;
        ClassContextBuilder builder = new ClassContextBuilder (this, targetType, parentContext);
        _classContextBuilders.Add (targetType, builder);
      }
      return _classContextBuilders[targetType];
    }

    /// <summary>
    /// Begins configuration of a target class.
    /// </summary>
    /// <typeparam name="TTargetType">The class to be configured.</typeparam>
    /// <returns>A fluent interface object for configuring the given <typeparamref name="TTargetType"/>.</returns>
    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return ForClass (typeof (TTargetType));
    }

    /// <summary>
    /// Adds the given mixin to the given target type with a number of explicit dependencies and suppressed mixins. This is a shortcut
    /// method for calling <see cref="ForClass"/>, <see cref="ClassContextBuilder.AddMixin"/>, <see cref="MixinContextBuilder.WithDependencies"/>,
    /// and <see cref="MixinContextBuilder.SuppressMixins"/> in a row.
    /// </summary>
    /// <param name="mixinKind">The kind of relationship the mixin has with its target class.</param>
    /// <param name="targetType">The target type to add a mixin for.</param>
    /// <param name="mixinType">The mixin type to add.</param>
    /// <param name="introducedMemberVisibility">The default visibility to be used for introduced members.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin in the context of the target type.</param>
    /// <param name="suppressedMixins">The mixins suppressed by this mixin in the context of the target type.</param>
    public virtual MixinConfigurationBuilder AddMixinToClass (MixinKind mixinKind, Type targetType, Type mixinType, MemberVisibility introducedMemberVisibility, IEnumerable<Type> explicitDependencies, IEnumerable<Type> suppressedMixins)
    {
      MixinContextBuilder mixinContextBuilder = AddMixinToClass (targetType, mixinType);
      CheckForSelfSuppressor (targetType, mixinType, suppressedMixins);

      mixinContextBuilder
          .OfKind (mixinKind)
          .WithDependencies (EnumerableUtility.ToArray (explicitDependencies))
          .WithIntroducedMemberVisibility (introducedMemberVisibility)
          .SuppressMixins (EnumerableUtility.ToArray (suppressedMixins));

      return this;
    }

    private void CheckForSelfSuppressor (Type targetType, Type mixinType, IEnumerable<Type> suppressedMixins)
    {
      foreach (Type suppressedMixinType in suppressedMixins)
      {
        if (ReflectionUtility.CanAscribe (mixinType, suppressedMixinType))
        {
          string message = string.Format ("Mixin type {0} applied to target class {1} suppresses itself.", mixinType.FullName,
              targetType.FullName);
          throw new InvalidOperationException (message);
        }
      }
    }

    private MixinContextBuilder AddMixinToClass (Type targetType, Type mixinType)
    {
      MixinContextBuilder mixinContextBuilder;
      try
      {
        mixinContextBuilder = ForClass (targetType).AddMixin (mixinType);
      }
      catch (ArgumentException ex)
      {
        Type typeForMessage = mixinType;
        if (typeForMessage.IsGenericType)
          typeForMessage = typeForMessage.GetGenericTypeDefinition ();
        string message = string.Format (
            "Two instances of mixin {0} are configured for target type {1}.",
            typeForMessage.FullName,
            targetType.FullName);
        throw new ConfigurationException (message, ex);
      }
      return mixinContextBuilder;
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      IEnumerable<ClassContext> parentContexts = ParentConfiguration != null ?
          (IEnumerable<ClassContext>) ParentConfiguration.ClassContexts : new ClassContext[0];
      MixinConfiguration builtConfiguration = new MixinConfiguration (ParentConfiguration);
      InheritanceAwareMixinConfigurationBuilder builder = new InheritanceAwareMixinConfigurationBuilder (builtConfiguration, parentContexts, ClassContextBuilders);
      builtConfiguration = builder.BuildMixinConfiguration();
      return builtConfiguration;
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      MixinConfiguration configuration = BuildConfiguration();
      return configuration.EnterScope();
    }
  }
}
