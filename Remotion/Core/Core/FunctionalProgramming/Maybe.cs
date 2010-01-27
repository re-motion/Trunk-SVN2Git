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
using System.Collections.Generic;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.FunctionalProgramming
{
  /// <summary>
  /// Provides non-generic helper methods for the <see cref="Maybe{T}"/> type.
  /// </summary>
  public static class Maybe
  {
    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> instance for the given value, which can be <see langword="null" />.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="valueOrNull"/>.</typeparam>
    /// <param name="valueOrNull">The value. Can be <see langword="null" />, in which case <see cref="Maybe{T}.Nothing"/> is returned.</param>
    /// <returns><see cref="Maybe{T}.Nothing"/> if <paramref name="valueOrNull"/> is <see langword="null" />; otherwise an instance of 
    /// <see cref="Maybe{T}"/> that encapsulates the given <paramref name="valueOrNull"/>.</returns>
    public static Maybe<T> ForValue<T> (T valueOrNull)
    {
      return new Maybe<T> (valueOrNull);
    }

    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> instance for the given <paramref name="nullableValue"/>, unwrapping a nullable value type.
    /// </summary>
    /// <typeparam name="T">The underlying type of the <paramref name="nullableValue"/>.</typeparam>
    /// <param name="nullableValue">
    ///   The nullable value. Can be <see langword="null"/>, in which case <see cref="Maybe{T}.Nothing"/> is returned.
    /// </param>
    /// <returns><see cref="Maybe{T}.Nothing"/> if <paramref name="nullableValue"/> is <see langword="null" />; otherwise an instance of 
    /// <see cref="Maybe{T}"/> that encapsulates the underlying value of <paramref name="nullableValue"/>.</returns>
    public static Maybe<T> ForValue<T> (T? nullableValue) where T : struct
    {
      return nullableValue.HasValue ? new Maybe<T> (nullableValue.Value) : Maybe<T>.Nothing;
    }

    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> instance for the given value, which can be <see langword="null"/>, if a boolean condition evaluates to
    /// <see langword="true"/>. If it evaluates to <see langword="false"/>, <see cref="Maybe{T}.Nothing"/> is returned.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="valueIfTrue"/>.</typeparam>
    /// <param name="condition">The condition to check. If <see langword="false" />, <see cref="Maybe{T}.Nothing"/> is returned.</param>
    /// <param name="valueIfTrue">The value. Can be <see langword="null"/>, in which case <see cref="Maybe{T}.Nothing"/> is returned.</param>
    /// <returns>
    /// 	<see cref="Maybe{T}.Nothing"/> if <paramref name="valueIfTrue"/> is <see langword="null"/> or <paramref name="condition"/> is 
    /// 	<see langword="false" />; otherwise an instance of <see cref="Maybe{T}"/> that encapsulates the given <paramref name="valueIfTrue"/>.
    /// </returns>
    public static Maybe<T> ForCondition<T> (bool condition, T valueIfTrue)
    {
      return ForValue (valueIfTrue).Where (v => condition);
    }

    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> instance for the given value, unwrapping a nullable value type, if a boolean condition evaluates to
    /// <see langword="true"/>. If it evaluates to <see langword="false"/>, <see cref="Maybe{T}.Nothing"/> is returned.
    /// </summary>
    /// <typeparam name="T">The underlying type of the <paramref name="nullableValueIfTrue"/>.</typeparam>
    /// <param name="condition">The condition to check. If <see langword="false" />, <see cref="Maybe{T}.Nothing"/> is returned.</param>
    /// <param name="nullableValueIfTrue">
    ///   The nullable value. Can be <see langword="null"/>, in which case <see cref="Maybe{T}.Nothing"/> is returned.
    ///   </param>
    /// <returns>
    /// 	<see cref="Maybe{T}.Nothing"/> if <paramref name="nullableValueIfTrue"/> is <see langword="null"/> or <paramref name="condition"/> is 
    /// 	<see langword="false" />; otherwise an instance of <see cref="Maybe{T}"/> that encapsulates the underlying value of 
    /// 	<paramref name="nullableValueIfTrue"/>.
    /// </returns>
    public static Maybe<T> ForCondition<T> (bool condition, T? nullableValueIfTrue) where T : struct
    {
      return condition ? ForValue (nullableValueIfTrue) : Maybe<T>.Nothing;
    }

    /// <summary>
    /// Enumerates the values of a number of <see cref="Maybe{T}"/> instances. <see cref="Maybe{T}"/> instances that have no values are ignored.
    /// </summary>
    /// <typeparam name="T">The value type of the <see cref="Maybe{T}"/> values.</typeparam>
    /// <param name="maybeValues">The maybe instances to enumerate the values of. <see cref="Maybe{T}"/> instances that have no values are ignored.</param>
    /// <returns>An enumerable sequence containing all non-<see langword="null" /> values</returns>
    public static IEnumerable<T> EnumerateValues<T> (IEnumerable<Maybe<T>> maybeValues)
    {
      return maybeValues.Where (v => v.HasValue).Select (v => v.ValueOrDefault ());
    }

    /// <summary>
    /// Enumerates the values of a number of <see cref="Maybe{T}"/> instances. <see cref="Maybe{T}"/> instances that have no values are ignored.
    /// </summary>
    /// <typeparam name="T">The value type of the <see cref="Maybe{T}"/> values.</typeparam>
    /// <param name="maybeValues">The maybe instances to enumerate the values of. <see cref="Maybe{T}"/> instances that have no values are ignored.</param>
    /// <returns>An enumerable sequence containing all non-<see langword="null" /> values</returns>
    public static IEnumerable<T> EnumerateValues<T> (params Maybe<T>[] maybeValues)
    {
      return EnumerateValues ((IEnumerable<Maybe<T>>) maybeValues);
    }
  }

  /// <summary>
  /// Encapsulates a value that may be <see langword="null" />, providing helpful methods to avoid <see langword="null" /> checks.
  /// </summary>
  public struct Maybe<T>
  {
    public static readonly Maybe<T> Nothing = new Maybe<T> ();

    private readonly T _value;
    private readonly bool _hasValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Maybe{T}"/> struct.
    /// </summary>
    /// <param name="value">
    /// The value. If the value is <see langword="null" />, the created instance will compare equal to <see cref="Nothing"/>.
    /// </param>
    public Maybe (T value)
    {
      _value = value;
      // ReSharper disable CompareNonConstrainedGenericWithNull
      _hasValue = value != null;
      // ReSharper restore CompareNonConstrainedGenericWithNull
    }

    /// <summary>
    /// Gets a value indicating whether this instance has a value.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has a value; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasValue
    {
      get { return _hasValue; }
    }

    /// <summary>
    /// Provides a human-readable representation of this instance.
    /// </summary>
    /// <returns>
    /// A human-readable representation of this instance.
    /// </returns>
    public override string ToString ()
    {
      return string.Format ("{0} ({1})", (HasValue ? "Value: " + _value : "Nothing"), typeof (T).Name);
    }

    /// <summary>
    /// Selects another value from this instance. If this instance does not have a value, the selected value is <see cref="Nothing"/>.
    /// Otherwise, the selected value is retrieved via a selector function.
    /// </summary>
    /// <typeparam name="TR">The type of the value to be selected.</typeparam>
    /// <param name="selector">The selector function. This function is only executed if this instance has a value.</param>
    /// <returns><see cref="Nothing"/> if this instance has no value or <paramref name="selector"/> returns <see langword="null" />; 
    /// otherwise, a new <see cref="Maybe{T}"/> instance holding the value returned by <paramref name="selector"/>.
    /// </returns>
    public Maybe<TR> Select<TR> (Func<T, TR> selector)
    {
      ArgumentUtility.CheckNotNull ("selector", selector);

      if (_hasValue)
        return Maybe.ForValue (selector (_value));
      else
        return Maybe<TR>.Nothing;
    }

    /// <summary>
    /// Selects a nullable value from this instance. If this instance does not have a value, the selected value is <see cref="Nothing"/>.
    /// Otherwise, the selected value is retrieved via a selector function.
    /// </summary>
    /// <typeparam name="TR">The type of the value to be selected.</typeparam>
    /// <param name="selector">The selector function. This function is only executed if this instance has a value. Its return value is unwrapped
    /// into the underlying type.</param>
    /// <returns><see cref="Nothing"/> if this instance has no value or <paramref name="selector"/> returns <see langword="null" />; 
    /// otherwise, a new <see cref="Maybe{T}"/> instance holding the non-nullable value returned by <paramref name="selector"/>.
    /// </returns>
    public Maybe<TR> Select<TR> (Func<T, TR?> selector) where TR : struct
    {
      ArgumentUtility.CheckNotNull ("selector", selector);

      if (_hasValue)
        return Maybe.ForValue (selector (_value));
      else
        return Maybe<TR>.Nothing;
    }

    /// <summary>
    /// Gets the value held by this instance, or the default value of <typeparamref name="T"/> if this instance does not have a value.
    /// </summary>
    /// <returns>The value held by this instance, or the default value of <typeparamref name="T"/> if this instance does not have a value.</returns>
    public T ValueOrDefault ()
    {
      return ValueOrDefault (default(T));
    }

    /// <summary>
    /// Gets the value held by this instance, or the <paramref name="defaultValue"/> if this instance does not have a value.
    /// </summary>
    /// <param name="defaultValue">The default value returned if this instance does not have a value.</param>
    /// <returns>The value held by this instance, or the <paramref name="defaultValue"/> if this instance does not have a value.</returns>
    public T ValueOrDefault (T defaultValue)
    {
      return _hasValue ? _value : defaultValue;
    }

    /// <summary>
    /// Gets the value held by this instance. An exception is thrown if this instance does not have a value.
    /// </summary>
    /// <returns>The value held by this instance. An exception is thrown if this instance does not have a value.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="HasValue"/> property is <see langword="false" />.</exception>
    public T Value ()
    {
      if (!_hasValue)
        throw new InvalidOperationException ("Maybe-Object must have a value.");
      return _value;
    }

    /// <summary>
    /// Executes the specified action if this instance has a value. Otherwise, the action is not performed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Do (Action<T> action)
    {
      if (_hasValue)
        action (_value);
    }

    public Maybe<T> Where (Func<T, bool> predicate)
    {
      return _hasValue && predicate (_value) ? this : Nothing;
    }
  }
}