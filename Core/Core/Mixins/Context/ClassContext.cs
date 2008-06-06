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
using System.Runtime.Serialization;
using System.Text;
using Remotion.Mixins.Utilities.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Holds the mixin configuration information for a single mixin target class.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// Instances of this class are immutable.
  /// </remarks>
  [Serializable]
  public sealed class ClassContext : ISerializable
  {
    private static IEnumerable<MixinContext> GetMixinContexts (Type[] mixinTypes)
    {
      Dictionary<Type, MixinContext> mixins = new Dictionary<Type, MixinContext> (mixinTypes.Length);
      foreach (Type mixinType in mixinTypes)
      {
        if (!mixins.ContainsKey (mixinType))
        {
          MixinContext context = new MixinContext (MixinKind.Extending, mixinType, new Type[0]);
          mixins.Add (context.MixinType, context);
        }
        else
        {
          string message = string.Format ("The mixin type {0} was tried to be added twice.", mixinType.FullName);
          throw new ArgumentException (message, "mixinTypes");
        }
      }
      return mixins.Values;
    }

    private static int CalculateHashCode (ClassContext classContext)
    {
      return classContext.Type.GetHashCode ()
          ^ EqualityUtility.GetXorHashCode (classContext.Mixins)
          ^ EqualityUtility.GetXorHashCode (classContext.CompleteInterfaces);
    }

    private readonly Type _type;
    private readonly MixinContextCollection _mixins;
    private readonly ReadOnlyContextCollection<Type, Type> _completeInterfaces;
    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given mixin target type.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <param name="mixins">A list of <see cref="MixinContext"/> objects representing the mixins applied to this class.</param>
    /// <param name="completeInterfaces">The complete interfaces supported by the class.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type, IEnumerable<MixinContext> mixins, IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;

      _mixins = new MixinContextCollection (mixins);
      _completeInterfaces = new ReadOnlyContextCollection<Type, Type> (delegate (Type t) { return t; }, completeInterfaces);

      _cachedHashCode = CalculateHashCode (this);
    }

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given target type without mixins.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type)
      : this (type, new MixinContext[0], new Type[0])
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given mixin target type.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <param name="mixins">A list of <see cref="MixinContext"/> objects representing the mixins applied to this class.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    public ClassContext (Type type, params MixinContext[] mixins)
        : this (type, mixins, new Type[0])
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ClassContext"/> for a given mixin target type and initializes it to be associated with the given
    /// mixin types.
    /// </summary>
    /// <param name="type">The mixin target type to be represented by this context.</param>
    /// <param name="mixinTypes">The mixin types to be associated with this context.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="mixinTypes"/> parameter contains duplicates.</exception>
    public  ClassContext (Type type, params Type[] mixinTypes)
        : this (type, GetMixinContexts (ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes)), new Type[0])
    {
    }

    /// <summary>
    /// Gets the type represented by this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The type represented by this context.</value>
    public Type Type
    {
      get { return _type; }
    }

    /// <summary>
    /// Gets the mixins associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The mixins associated with this context.</value>
    public MixinContextCollection Mixins
    {
      get { return _mixins; }
    }

    /// <summary>
    /// Gets the complete interfaces associated with this <see cref="ClassContext"/>.
    /// </summary>
    /// <value>The complete interfaces associated with this context (for an explanation, see <see cref="CompleteInterfaceAttribute"/>).</value>
    public ReadOnlyContextCollection<Type, Type> CompleteInterfaces
    {
      get { return _completeInterfaces; }
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"></see> to compare with this <see cref="ClassContext"/>.</param>
    /// <returns>
    /// True if the specified <see cref="T:System.Object"></see> is a <see cref="ClassContext"/> for the same type with equal mixin 
    /// and complete interfaces configuration; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      ClassContext other = obj as ClassContext;
      if (other == null)
        return false;
      
        if (!other.Type.Equals (Type) || other._mixins.Count != _mixins.Count || other._completeInterfaces.Count != _completeInterfaces.Count)
          return false;

        foreach (MixinContext mixinContext in _mixins)
        {
          if (!other._mixins.ContainsKey (mixinContext.MixinType) || !other._mixins[mixinContext.MixinType].Equals (mixinContext))
            return false;
        }

        foreach (Type completeInterface in _completeInterfaces)
        {
          if (!other._completeInterfaces.ContainsKey (completeInterface))
            return false;
        }

      return true;
    }

    /// <summary>
    /// Returns a hash code for this <see cref="ClassContext"/>.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="ClassContext"/> which includes the hash codes of this object's complete interfaces and mixin ocntexts.
    /// </returns>
    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="ClassContext"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"></see> containing the type names of this context's associated <see cref="Type"/>, all its mixin types, and
    /// complete interfaces.
    /// </returns>
    public override string ToString ()
    {
      StringBuilder sb = new StringBuilder (Type.FullName);
      foreach (MixinContext mixinContext in Mixins)
        sb.Append (" + ").Append (mixinContext.MixinType.FullName);
      foreach (Type completeInterfaceType in CompleteInterfaces)
        sb.Append (" => ").Append (completeInterfaceType.FullName);
      return sb.ToString();
    }

    /// <summary>
    /// Returns a new <see cref="ClassContext"/> with the same mixins and complete interfaces as this object, but a different target type.
    /// </summary>
    /// <param name="type">The target type to create the new <see cref="ClassContext"/> for.</param>
    /// <returns>A clone of this <see cref="ClassContext"/> for a different target type.</returns>
    public ClassContext CloneForSpecificType (Type type)
    {
      ClassContext newInstance = new ClassContext (type, Mixins, CompleteInterfaces);
      return newInstance;
    }

    /// <summary>
    /// Creates a clone of the current class context, replacing its generic parameters with type arguments. This method is only allowed on
    /// class contexts representing a generic type definition.
    /// </summary>
    /// <param name="genericArguments">The type arguments to specialize this context's <see cref="Type"/> with.</param>
    /// <returns>A <see cref="ClassContext"/> which is identical to this one except its <see cref="Type"/> being specialized with the
    /// given <paramref name="genericArguments"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="genericArguments"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><see cref="Type"/> is not a generic type definition.</exception>
    public ClassContext SpecializeWithTypeArguments (Type[] genericArguments)
    {
      ArgumentUtility.CheckNotNull ("genericArguments", genericArguments);

      if (!Type.IsGenericTypeDefinition)
        throw new InvalidOperationException ("This method is only allowed on generic type definitions.");

      return CloneForSpecificType (Type.MakeGenericType (genericArguments));
    }

    /// <summary>
    /// Creates a new <see cref="ClassContext"/> inheriting all data from the given <paramref name="baseContext"/> and applying overriding rules for
    /// mixins and concrete interfaces already defined for this <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="baseContext">The base context to inherit data from.</param>
    /// <returns>A new <see cref="ClassContext"/> combining the mixins of this object with those from the <paramref name="baseContext"/>.</returns>
    /// <exception cref="ConfigurationException">The <paramref name="baseContext"/> contains mixins whose base types or generic
    /// type definitions are already defined on this mixin. The derived context cannot have concrete mixins whose base types
    /// are defined on the parent context.
    /// </exception>
    public ClassContext InheritFrom (ClassContext baseContext)
    {
      ArgumentUtility.CheckNotNull ("baseContext", baseContext);
      return InheritFrom (new ClassContext[] {baseContext});
    }

    /// <summary>
    /// Creates a new <see cref="ClassContext"/> inheriting all data from the given <paramref name="baseContexts"/> and applying overriding rules for
    /// mixins and concrete interfaces already defined for this <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="baseContexts">The base contexts to inherit data from.</param>
    /// <returns>A new <see cref="ClassContext"/> combining the mixins of this object with those from the <paramref name="baseContexts"/>.</returns>
    /// <exception cref="ConfigurationException">The <paramref name="baseContexts"/> contain mixins whose base types or generic
    /// type definitions are already defined on this mixin. The derived context cannot have concrete mixins whose base types
    /// are defined on the parent context.
    /// </exception>
    public ClassContext InheritFrom (IEnumerable<ClassContext> baseContexts)
    {
      return ClassContextDeriver.Instance.DeriveContext (this, baseContexts);
    }

    /// <summary>
    /// Returns a new <see cref="ClassContext"/> equivalent to this object but with all mixins ascribable from the given
    /// <paramref name="mixinTypesToSuppress"/> removed.
    /// </summary>
    /// <param name="mixinTypesToSuppress">The mixin types to suppress.</param>
    /// <returns>A copy of this <see cref="ClassContext"/> without any mixins that can be ascribed to the given mixin types.</returns>
    public ClassContext SuppressMixins (IEnumerable<Type> mixinTypesToSuppress)
    {
      Dictionary<Type, MixinContext> mixinsAfterSuppression = new Dictionary<Type, MixinContext> ();
      foreach (MixinContext mixinContext in _mixins)
        mixinsAfterSuppression.Add (mixinContext.MixinType, mixinContext);

      foreach (Type suppressedType in mixinTypesToSuppress)
      {
        foreach (MixinContext mixin in Mixins)
        {
          if (ReflectionUtility.CanAscribe (mixin.MixinType, suppressedType))
            mixinsAfterSuppression.Remove (mixin.MixinType);
        }
      }
      return new ClassContext (Type, mixinsAfterSuppression.Values, CompleteInterfaces);
    }

    #region Serialization
    private ClassContext (SerializationInfo info, StreamingContext context)
      : this (ReflectionObjectSerializer.DeserializeType ("_type", info), DeserializeMixins (info), DeserializeCompleteInterfaces (info))
    {
    }

    private static IEnumerable<MixinContext> DeserializeMixins (SerializationInfo info)
    {
      int mixinCount = info.GetInt32 ("_mixins.Count");
      List<MixinContext> mixinContexts = new List<MixinContext> (mixinCount);
      for (int i = 0; i < mixinCount; ++i)
      {
        MixinContext mixinContext = MixinContextSerializer.DeserializeFromFlatStructure ("_mixins[" + i + "]", info);
        mixinContexts.Add (mixinContext);
      }
      return mixinContexts;
    }

    private static IEnumerable<Type> DeserializeCompleteInterfaces (SerializationInfo info)
    {
      int completeInterfaceCount = info.GetInt32 ("_completeInterfaces.Count");
      List<Type> completeInterfaces = new List<Type> (completeInterfaceCount);
      for (int i = 0; i < completeInterfaceCount; ++i)
        completeInterfaces.Add (ReflectionObjectSerializer.DeserializeType ("_completeInterfaces[" + i + "]", info));
      return completeInterfaces;
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      ReflectionObjectSerializer.SerializeType (_type, "_type", info);
      info.AddValue ("_mixins.Count", _mixins.Count);
      IEnumerator<MixinContext> mixinEnumerator = _mixins.GetEnumerator ();
      for (int i = 0; mixinEnumerator.MoveNext (); ++i)
        MixinContextSerializer.SerializeIntoFlatStructure (mixinEnumerator.Current, "_mixins[" + i + "]", info);

      IEnumerator<Type> interfaceEnumerator = _completeInterfaces.GetEnumerator ();
      info.AddValue ("_completeInterfaces.Count", _completeInterfaces.Count);
      for (int i = 0; interfaceEnumerator.MoveNext (); ++i)
        ReflectionObjectSerializer.SerializeType (interfaceEnumerator.Current, "_completeInterfaces[" + i + "]", info);
    }
    #endregion
  }
}
