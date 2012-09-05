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
using System.Text;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Context.Suppression;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Holds the mixin configuration information for a single mixin target class.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// Instances of this class are immutable.
  /// </remarks>
  public sealed class ClassContext
  {
    public static ClassContext Deserialize (IClassContextDeserializer deserializer)
    {
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);
      return new ClassContext (
          deserializer.GetClassType (),
          deserializer.GetMixins (),
          deserializer.GetCompleteInterfaces ());
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
      _completeInterfaces = new ReadOnlyContextCollection<Type, Type> (t => t, completeInterfaces);

      _cachedHashCode = CalculateHashCode (this);
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
    /// Determines whether this <see cref="ClassContext"/> is empty, i.e. it contains no <see cref="Mixins"/> or <see cref="CompleteInterfaces"/>.
    /// </summary>
    /// <returns>
    /// 	<see langword="true" /> if this <see cref="ClassContext"/> is empty; otherwise, <see langword="false" />.
    /// </returns>
    public bool IsEmpty ()
    {
      return Mixins.Count == 0 && CompleteInterfaces.Count == 0;
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
      if (object.ReferenceEquals (this, obj))
        return true;

      var other = obj as ClassContext;
      if (other == null)
        return false;

      if (other._cachedHashCode != _cachedHashCode 
          || !other.Type.Equals (Type) 
          || other._mixins.Count != _mixins.Count 
          || other._completeInterfaces.Count != _completeInterfaces.Count)
        return false;

      // No LINQ expression for performance reasons (avoid closure)
      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (var mixinContext in _mixins)
      {
        var otherMixinContext = other._mixins[mixinContext.MixinType];
        if (otherMixinContext == null || !otherMixinContext.Equals (mixinContext))
          return false;
      }

      foreach (var completeInterface in _completeInterfaces)
      {
        if (!other._completeInterfaces.ContainsKey (completeInterface))
          return false;
      }
      // ReSharper restore LoopCanBeConvertedToQuery

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
      var sb = new StringBuilder (Type.FullName);
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
      var newInstance = new ClassContext (type, Mixins, CompleteInterfaces);
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
      return InheritFrom (new[] {baseContext});
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
    /// Returns a new <see cref="ClassContext"/> equivalent to this object but with all mixins affected by the given 
    /// <paramref name="suppressionRules"/> removed.
    /// </summary>
    /// <param name="suppressionRules">The rules describing the mixin types to suppress.</param>
    /// <returns>
    /// A copy of this <see cref="ClassContext"/> without any mixins that are affected by the given <parmref name="suppressionRules"/>.
    /// </returns>
    public ClassContext SuppressMixins (IEnumerable<IMixinSuppressionRule> suppressionRules)
    {
      var mixinsAfterSuppression = _mixins.ToDictionary (mc => mc.MixinType);
      foreach (var rule in suppressionRules)
        rule.RemoveAffectedMixins (mixinsAfterSuppression);

      return new ClassContext (Type, mixinsAfterSuppression.Values, CompleteInterfaces);
    }

    public void Serialize (IClassContextSerializer serializer)
    {
      ArgumentUtility.CheckNotNull ("serializer", serializer);

      serializer.AddClassType (Type);
      serializer.AddMixins (Mixins);
      serializer.AddCompleteInterfaces (CompleteInterfaces);
    }
  }
}
