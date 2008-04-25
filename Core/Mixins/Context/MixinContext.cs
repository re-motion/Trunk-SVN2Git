using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Utilities.Serialization;
using Remotion.Utilities;
using Remotion.Collections;

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
    internal static MixinContext DeserializeFromFlatStructure (string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      Type mixinType = ReflectionObjectSerializer.DeserializeType (key + ".MixinType", info);

      int dependencyCount = info.GetInt32 (key + ".ExplicitDependencyCount");
      List<Type> explicitDependencies = new List<Type>();
      for (int i = 0; i < dependencyCount; ++i)
        explicitDependencies.Add (ReflectionObjectSerializer.DeserializeType (key + ".ExplicitDependencies[" + i + "]", info));

      MixinContext newContext = new MixinContext (mixinType, explicitDependencies);
      return newContext;
    }

    public readonly Type MixinType;
    private readonly ReadOnlyContextCollection<Type, Type> _explicitDependencies;
    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixinContext"/> class.
    /// </summary>
    /// <param name="mixinType">The mixin type represented by this <see cref="MixinContext"/>.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin.</param>
    public MixinContext (Type mixinType, IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);

      MixinType = mixinType;
      _explicitDependencies = new ReadOnlyContextCollection<Type, Type> (delegate (Type t) { return t; }, explicitDependencies);

      _cachedHashCode = MixinType.GetHashCode () ^ EqualityUtility.GetXorHashCode (ExplicitDependencies);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MixinContext"/> class.
    /// </summary>
    /// <param name="mixinType">The mixin type represented by this <see cref="MixinContext"/>.</param>
    /// <param name="explicitDependencies">The explicit dependencies of the mixin.</param>
    public MixinContext (Type mixinType, params Type[] explicitDependencies)
        : this (mixinType, (IEnumerable<Type>) explicitDependencies)
    {
    }

    internal void SerializeIntoFlatStructure (string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      ReflectionObjectSerializer.SerializeType (MixinType, key + ".MixinType", info);
      info.AddValue (key + ".ExplicitDependencyCount", ExplicitDependencies.Count);
      IEnumerator<Type> dependencyEnumerator = ExplicitDependencies.GetEnumerator();
      for (int i = 0; dependencyEnumerator.MoveNext(); ++i)
        ReflectionObjectSerializer.SerializeType (dependencyEnumerator.Current, key + ".ExplicitDependencies[" + i + "]", info);
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
      
      if (!other.MixinType.Equals (MixinType) || other.ExplicitDependencies.Count != this.ExplicitDependencies.Count)
        return false;

      foreach (Type explicitDependency in ExplicitDependencies)
        if (!other.ExplicitDependencies.ContainsKey (explicitDependency))
          return false;
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
      get
      {
        return _explicitDependencies;
      }
    }
  }
}
