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
using System.Linq;
using Remotion.Mixins.Context.Serialization;
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
    public static MixinContext Deserialize (IMixinContextDeserializer deserializer)
    {
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);
      return new MixinContext (
          deserializer.GetMixinKind (),
          deserializer.GetMixinType (),
          deserializer.GetIntroducedMemberVisibility (),
          deserializer.GetExplicitDependencies (),
          deserializer.GetOrigin()
        );
    }

    private readonly Type _mixinType;
    private readonly MixinKind _mixinKind;
    private readonly MemberVisibility _introducedMemberVisibility;
    private readonly ReadOnlyContextCollection<Type, Type> _explicitDependencies;
    private readonly MixinContextOrigin _origin;

    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixinContext"/> class.
    /// </summary>
    /// <param name="mixinKind">The kind of relationship the configured mixin has with its target class.</param>
    /// <param name="mixinType">The mixin type represented by this <see cref="MixinContext"/>.</param>
    /// <param name="introducedMemberVisibility">The default visbility of introduced members.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin.</param>
    /// <param name="origin">
    /// A description of where the <see cref="MixinContext"/> originates from. Note that <paramref name="origin"/> is not considered when comparing 
    /// <see cref="MixinContext"/> objects for equality.
    /// </param>
    public MixinContext (
        MixinKind mixinKind,
        Type mixinType,
        MemberVisibility introducedMemberVisibility,
        IEnumerable<Type> explicitDependencies,
        MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      ArgumentUtility.CheckNotNull ("origin", origin);

      _mixinType = mixinType;
      _mixinKind = mixinKind;
      _introducedMemberVisibility = introducedMemberVisibility;
      _explicitDependencies = new ReadOnlyContextCollection<Type, Type> (t => t, explicitDependencies);
      _origin = origin;

      _cachedHashCode = EqualityUtility.GetRotatedHashCode (
          _mixinKind,
          _mixinType,
          EqualityUtility.GetXorHashCode (ExplicitDependencies),
          IntroducedMemberVisibility);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="MixinContext"/>. The 
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"></see> to compare with this <see cref="MixinContext"/>.</param>
    /// <returns>
    /// True if the specified <see cref="T:System.Object"></see> is a <see cref="MixinContext"/> for the same mixin type with equal explicit
    /// dependencies; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      var other = obj as MixinContext;
      if (other == null)
        return false;
      
      if (other._mixinKind != _mixinKind 
        || other._mixinType != _mixinType 
        || other.IntroducedMemberVisibility != IntroducedMemberVisibility
        || other.ExplicitDependencies.Count != ExplicitDependencies.Count)
        return false;

      return ExplicitDependencies.All (explicitDependency => other.ExplicitDependencies.ContainsKey (explicitDependency));
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
    /// The mixin type represented by the <see cref="MixinContext"/>.
    /// </summary>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    /// <summary>
    /// The kind of relationship the configured mixin has with its target class.
    /// </summary>
    public MixinKind MixinKind
    {
      get { return _mixinKind; }
    }

    /// <summary>
    /// Gets the default visibility of members introduced by this mixin.
    /// </summary>
    /// <value>The default introduced member visibility.</value>
    public MemberVisibility IntroducedMemberVisibility
    {
      get { return _introducedMemberVisibility; }
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
    /// Gets a description of where this <see cref="MixinContext"/> originates from. This member is not considered when comparing two 
    /// <see cref="MixinContext"/> instances for equality.
    /// </summary>
    /// <value>The <see cref="MixinContext"/>'s origin.</value>
    public MixinContextOrigin Origin
    {
      get { return _origin; }
    }

    public void Serialize (IMixinContextSerializer serializer)
    {
      ArgumentUtility.CheckNotNull ("serializer", serializer);

      serializer.AddMixinType (_mixinType);
      serializer.AddMixinKind (_mixinKind);
      serializer.AddIntroducedMemberVisibility (_introducedMemberVisibility);
      serializer.AddExplicitDependencies (_explicitDependencies);
      serializer.AddOrigin (_origin);
    }
  }
}
