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

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Represents a single mixin applied to a target class.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// Instances of this class are immutable.
  /// </remarks>
  public class MixinContext
  {
    /// <summary>
    /// The mixin type represented by the <see cref="MixinContext"/>.
    /// </summary>
    public readonly Type MixinType;

    /// <summary>
    /// The kind of relationship the configured mixin has with its target class.
    /// </summary>
    public readonly MixinKind MixinKind;

    private readonly ReadOnlyContextCollection<Type, Type> _explicitDependencies;
    private readonly MemberVisibility _introducedMemberVisibility;

    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixinContext"/> class.
    /// </summary>
    /// <param name="mixinKind">The kind of relationship the configured mixin has with its target class.</param>
    /// <param name="mixinType">The mixin type represented by this <see cref="MixinContext"/>.</param>
    /// <param name="introducedMemberVisibility">The default visbility of introduced members.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin.</param>
    public MixinContext (MixinKind mixinKind, Type mixinType, MemberVisibility introducedMemberVisibility, IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);

      MixinType = mixinType;
      MixinKind = mixinKind;
      _introducedMemberVisibility = introducedMemberVisibility;

      _explicitDependencies = new ReadOnlyContextCollection<Type, Type> (delegate (Type t) { return t; }, explicitDependencies);

      _cachedHashCode = EqualityUtility.GetRotatedHashCode (MixinKind, MixinType, EqualityUtility.GetXorHashCode (ExplicitDependencies), 
          IntroducedMemberVisibility);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixinContext"/> class.
    /// </summary>
    /// <param name="mixinKind">The kind of mixin represented by this <see cref="MixinContext"/>.</param>
    /// <param name="mixinType">The mixin type represented by this <see cref="MixinContext"/>.</param>
    /// <param name="introducedMemberVisibility">The default visbility of introduced members.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin.</param>
    public MixinContext (MixinKind mixinKind, Type mixinType, MemberVisibility introducedMemberVisibility, params Type[] explicitDependencies)
        : this (mixinKind, mixinType, introducedMemberVisibility, (IEnumerable<Type>) explicitDependencies)
    {
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="MixinContext"/>.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"></see> to compare with this <see cref="MixinContext"/>.</param>
    /// <returns>
    /// True if the specified <see cref="T:System.Object"></see> is a <see cref="MixinContext"/> for the same mixin type with equal explicit
    /// dependencies; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      MixinContext other = obj as MixinContext;
      if (other == null)
        return false;
      
      if (other.MixinKind != MixinKind 
        || other.MixinType != MixinType 
        || other.IntroducedMemberVisibility != IntroducedMemberVisibility
        || other.ExplicitDependencies.Count != ExplicitDependencies.Count)
        return false;

      foreach (Type explicitDependency in ExplicitDependencies)
      {
        if (!other.ExplicitDependencies.ContainsKey (explicitDependency))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Returns a hash code for this <see cref="MixinContext"/>.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="MixinContext"/> which includes the hash codes of its mixin type and its explicit dependencies.
    /// </returns>
    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    /// <summary>
    /// Gets the explicit dependencies added to this <see cref="MixinContext"/>.
    /// </summary>
    /// <value>The explicit dependencies added to this <see cref="MixinContext"/>.</value>
    /// <remarks>An explicit dependency is a base call dependency which should be considered for a mixin even though it is not expressed in the
    /// mixin's class declaration. This can be used to define the ordering of mixins in specific mixin configurations.</remarks>
    public ReadOnlyContextCollection<Type, Type> ExplicitDependencies
    {
      get { return _explicitDependencies; }
    }


    /// <summary>
    /// Gets the default visibility of members introduced by this mixin.
    /// </summary>
    /// <value>The default introduced member visibility.</value>
    public MemberVisibility IntroducedMemberVisibility
    {
      get { return _introducedMemberVisibility; }
    }
  }
}
