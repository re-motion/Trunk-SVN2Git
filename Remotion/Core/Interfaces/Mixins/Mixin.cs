// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Diagnostics;
using Remotion.Mixins.BridgeInterfaces;
using Remotion.ServiceLocation;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides reflective access to the mixins integrated with a target class.
  /// </summary>
  public static class Mixin
  {
    // This class holds lazy, readonly static fields. It relies on the fact that the .NET runtime will reliably initialize fields in a nested static
    // class with a static constructor as lazily as possible on first access of the static field.
    // Singleton implementations with nested classes are documented here: http://csharpindepth.com/Articles/General/Singleton.aspx.
    static class LazyStaticFields
    {
      public static readonly IMixinImplementation MixinImplementation = SafeServiceLocator.Current.GetInstance<IMixinImplementation> ();

      // ReSharper disable EmptyConstructor
      // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit; this will make the static fields as lazy as possible.
      static LazyStaticFields ()
      {
      }
      // ReSharper restore EmptyConstructor
    }

    /// <summary>
    /// Gets the instance of the specified mixin type <typeparamref name="TMixin"/> that was mixed into the given <paramref name="mixinTarget"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to get an instance of.</typeparam>
    /// <param name="mixinTarget">The mixin target to get the mixin instance from.</param>
    /// <returns>The instance of the specified mixin type that was mixed into the given mixin target, or <see langword="null"/> if the target does not
    /// include a mixin of that type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="mixinTarget"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method cannot be used with mixins that have been configured as open generic type definitions. Use the <see cref="Get(Type, object)">
    /// non-generic</see> variant instead.
    /// </remarks>
    public static TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      return LazyStaticFields.MixinImplementation.Get<TMixin> (mixinTarget);
    }

    /// <summary>
    /// Gets the instance of the specified <paramref name="mixinType"/> that was mixed into the given <paramref name="mixinTarget"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to get an instance of.</param>
    /// <param name="mixinTarget">The mixin target to get the mixin instance from.</param>
    /// <returns>The instance of the specified mixin type that was mixed into the given mixin target, or <see langword="null"/> if the target does not
    /// include a mixin of that type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> or the <paramref name="mixinTarget"/> parameter is
    /// <see langword="null"/>.</exception>
    /// <remarks>
    /// This method can also be used with mixins that have been configured as open generic type definitions. Use the open generic type definition
    /// to retrieve them, but be prepared to get an instance of a specialized (closed) generic type back.
    /// </remarks>
    public static object Get (Type mixinType, object mixinTarget)
    {
      return LazyStaticFields.MixinImplementation.Get (mixinType, mixinTarget);
    }
  }

  /// <summary>
  /// Base class for mixins that require a reference to their target object (<see cref="Mixin{TThis}.This"/>) and a reference for making base calls
  /// (<see cref="Base"/>).
  /// </summary>
  /// <typeparam name="TThis">The minimum type required for calling methods on the target object (<see cref="Mixin{TThis}.This"/>).</typeparam>
  /// <typeparam name="TBase">The minimum type required for making base calls (<see cref="Base"/>) when overriding a method of the target class.</typeparam>
  /// <remarks>
  /// <para>
  /// Typically, this base class will be used whenever a mixin overrides a method of a target class and it needs to call the overridden base implementation.
  /// Derive from the <see cref="Mixin{TThis}"/> class if you only need the target object reference but are not making any base calls, or use any
  /// base class if not even the target object reference is required.
  /// </para>
  /// <para>
  /// <typeparamref name="TThis"/> is called the face type requirement or This-dependency of the mixin, and can be assigned a class or interface (or
  /// a type parameter with class or interface constraints).
  /// </para>
  /// <para>
  /// <typeparamref name="TBase"/> is also called the base call type requirement or Base-dependency of the mixin and can be assigned an interface or
  /// the type  <see cref="System.Object"/> (or a type parameter with interface or <see cref="System.Object"/> constraints). The Base-dependencies
  /// of a mixin define the order in which method overrides are executed when multiple mixins override the same target method: when mixin A has a
  /// Base-dependency on an interface IB, its override will be executed before any mixin implementing the interface IB.
  /// </para>
  /// <para>
  /// If a subclass of this class is also a generic type (with at most two generic type parameters, each of which is bound to one of this class' generic
  /// type parameters), it can be configured as a mixin in its open generic type definition form (<c>typeof (C&lt;,&gt;)</c>). In such a case, the mixin
  /// engine will try to close it at the time of configuration analysis; for this to succeed, the following rules must apply:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// <para>
  /// The type parameter bound to <typeparamref name="TThis"/> must have at most one type constraint. The mixin engine will try to assign the target type of the
  /// mixin to this parameter when analyzing the configuration, if allowed by the constraints. A mixin writer can use this for introducing interfaces
  /// as follows:
  /// </para>
  /// <code>
  /// class MyMixin&lt;T&gt; : Mixin&lt;T, object&gt;, IEquatable&lt;T&gt;
  ///     where T : class
  /// {
  /// }
  /// </code>
  /// <para>
  /// In this example, the mixin will introduce the <see cref="IEquatable{T}"/> interface for its target class T.
  /// </para>
  /// <para>
  /// If the mixin engine cannot bind the type parameter to the target type (e.g. because the parameter has an incompatible type constaint), it will
  /// bind it to a type compatible with the parameter's constraint (or <see cref="System.Object"/> if no constraint exists) and the mixin implementer
  /// should not depend on this type parameter when introducing interfaces. If the constraint (if any) is not satisfied by the interfaces introduced
  /// via the mixins applied to the target type, the mixin configuration is invalid.
  /// </para>
  /// </item>
  /// <item>
  /// The type parameter bound to <typeparamref name="TBase"/> must have at most one type constraint. The mixin engine will assign a type compatible with this
  /// constraint (or <see cref="System.Object"/> if no constraint exists) to the parameter when analyzing the configuration, and the mixin implementer
  /// should not depend on this type parameter when introducing interfaces. If the constraint (if any) is not satisfied by the interfaces introduced
  /// via the mixins applied to the target type, the mixin configuration is invalid.
  /// </item>
  /// </list>
  /// </remarks>
  [Serializable]
  public class Mixin<TThis, TBase> : Mixin<TThis>, IInitializableMixin
      where TThis: class
      where TBase: class
  {
    [NonSerialized]
    private TBase _base;

    /// <summary>
    /// Gets an object reference for performing base calls from member overrides.
    /// </summary>
    /// <value>The base call object reference.</value>
    /// <exception cref="InvalidOperationException">The mixin has not been initialized yet, probably because the property is accessed from the mixin's
    /// constructor.</exception>
    /// <remarks>
    /// <para>
    /// This property must not be accessed from the mixin's constructor; if you need to initialize the mixin by accessing the <see cref="Base"/>
    /// property, override the <see cref="Mixin{TThis}.OnInitialized"/> method.
    /// </para>
    /// <para>
    /// This property is only for performing base calls from overridden methods. It should not be used to access arbitrary methods of the mixin's 
    /// target object, use the <see cref="Mixin{TThis}.This"/> property for instead. If the <see cref="Base"/> property is used other than to perform
    /// base calls from member overrides and more than one mixin is applied to the target object, unexpected behavior may arise because the 
    /// <see cref="Base"/> property will represent the target object together with only a subset, but not all of its mixins.
    /// </para>
    /// </remarks>
    protected TBase Base
    {
      [DebuggerStepThrough]
      get
      {
        if (_base == null)
          throw new InvalidOperationException ("Mixin has not been initialized yet.");
        return _base;
      }
    }

    void IInitializableMixin.Initialize (object @this, object @base, bool deserialization)
    {
      _this = (TThis) @this;
      _base = (TBase) @base;
      if (deserialization)
        OnDeserialized ();
      else
        OnInitialized ();
    }
  }

  /// <summary>
  /// Base class for mixins that require a reference to their target object (<see cref="This"/>).
  /// </summary>
  /// <typeparam name="TThis">The minimum type required for calling methods on the target object (<see cref="This"/>).</typeparam>
  /// <remarks>
  /// <para>
  /// Typically, this base class will be used for those mixins which do require a reference to their target object, but which do not overrride
  /// any methods. 
  /// Derive from the <see cref="Mixin{TThis, TBase}"/> class if you need to override target methods, or use any
  /// base class if not even the target object reference is required.
  /// </para>
  /// <para>
  /// <typeparamref name="TThis"/> is called the face type requirement or This-dependency of the mixin, and can be assigned a class or interface (or
  /// a type parameter with class or interface constraints).
  /// </para>
  /// <para>
  /// If a subclass of this class is also a generic type (with at most one generic type parameter, which is bound to the <typeparamref name="TThis"/>
  /// type parameter), it can be configured as a mixin in its open generic type definition form (<c>typeof (C&lt;&gt;)</c>). In such a case, the mixin
  /// engine will try to close it at the time of configuration analysis; for this to succeed, the following rule must apply:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// <para>
  /// The type parameter bound to <typeparamref name="TThis"/> must have at most one type constraint. The mixin engine will try to assign the target type of the
  /// mixin to this parameter when analyzing the configuration, if allowed by the constraints. A mixin writer can use this for introducing interfaces
  /// as follows:
  /// </para>
  /// <code>
  /// class MyMixin&lt;T&gt; : Mixin&lt;T&gt;, IEquatable&lt;T&gt;
  ///     where T : class
  /// {
  /// }
  /// </code>
  /// <para>
  /// In this example, the mixin will introduce the <see cref="IEquatable{T}"/> interface for its target class T.
  /// </para>
  /// <para>
  /// If the mixin engine cannot bind the type parameter to the target type (e.g. because the parameter has an incompatible type constaint), it will
  /// bind it to a type compatible with the parameter's constraint (or <see cref="System.Object"/> if no constraint exists), and the mixin implementer
  /// should not depend on this type parameter when introducing interfaces. If the constraint (if any) is not satisfied by the interfaces introduced
  /// via the mixins applied to the target type, the mixin configuration is invalid.
  /// </para>
  /// </item>
  /// </list>
  /// </remarks>
  [Serializable]
  public class Mixin<TThis> : IInitializableMixin
      where TThis: class
  {
    [NonSerialized]
    internal TThis _this;

    /// <summary>
    /// Gets a reference to the concrete mixed object.
    /// </summary>
    /// <value>The target object reference.</value>
    /// <exception cref="InvalidOperationException">The mixin has not been initialized yet, probably because the property is accessed from the mixin's
    /// constructor.</exception>
    /// <remarks>
    /// This property must not be accessed from the mixin's constructor; if you need to initialize the mixin by accessing the <see cref="This"/>
    /// property, override the <see cref="Mixin{TThis}.OnInitialized"/> method.
    /// <note type="warning">
    /// Be careful when calling members that this mixin overrides via the <see cref="This"/> property, this can easily throw a
    /// <see cref="StackOverflowException"/> because the <see cref="This"/> property includes all mixins defined on the target object. Use 
    /// <see cref="Mixin{TThis,TBase}.Base"/> instead to call the base implementations of overridden members.
    /// </note>
    /// </remarks>
    protected TThis This
    {
      [DebuggerStepThrough]
      get
      {
        if (_this == null)
          throw new InvalidOperationException ("Mixin has not been initialized yet.");
        return _this;
      }
    }

    /// <summary>
    /// Called when the mixin has been initialized and its properties can be safely accessed.
    /// </summary>
    protected virtual void OnInitialized()
    {
      // nothing
    }

    /// <summary>
    /// Called when the mixin has been deserialized and its properties can be safely accessed.
    /// </summary>
    protected virtual void OnDeserialized ()
    {
      // nothing
    }

    void IInitializableMixin.Initialize (object @this, object @base, bool deserialization)
    {
      _this = (TThis) @this;
      if (deserialization)
        OnDeserialized();
      else
        OnInitialized();
    }
  }
}
