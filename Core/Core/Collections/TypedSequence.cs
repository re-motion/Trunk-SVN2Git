// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.Collections
{
  /// <summary>
  /// Stores a sequence of values together with their types.
  /// </summary>
  public class TypedSequence : IEnumerable<Tuple<Type, object>>
  {
    /// <summary>
    /// An empty sequence.
    /// </summary>
    public readonly static TypedSequence Empty = new TypedSequence ();

    private readonly Tuple<Type, object>[] _typedValues;

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1> (A1 a1)
    {
      return new TypedSequence (
          Tuple.NewTuple( typeof (A1), (object) a1)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2> (A1 a1, A2 a2)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3),
          Tuple.NewTuple (typeof (A4), (object) a4)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3),
          Tuple.NewTuple (typeof (A4), (object) a4),
          Tuple.NewTuple (typeof (A5), (object) a5)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3),
          Tuple.NewTuple (typeof (A4), (object) a4),
          Tuple.NewTuple (typeof (A5), (object) a5),
          Tuple.NewTuple (typeof (A6), (object) a6)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3),
          Tuple.NewTuple (typeof (A4), (object) a4),
          Tuple.NewTuple (typeof (A5), (object) a5),
          Tuple.NewTuple (typeof (A6), (object) a6),
          Tuple.NewTuple (typeof (A7), (object) a7)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3),
          Tuple.NewTuple (typeof (A4), (object) a4),
          Tuple.NewTuple (typeof (A5), (object) a5),
          Tuple.NewTuple (typeof (A6), (object) a6),
          Tuple.NewTuple (typeof (A7), (object) a7),
          Tuple.NewTuple (typeof (A8), (object) a8)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1),
          Tuple.NewTuple (typeof (A2), (object) a2),
          Tuple.NewTuple (typeof (A3), (object) a3),
          Tuple.NewTuple (typeof (A4), (object) a4),
          Tuple.NewTuple (typeof (A5), (object) a5),
          Tuple.NewTuple (typeof (A6), (object) a6),
          Tuple.NewTuple (typeof (A7), (object) a7),
          Tuple.NewTuple (typeof (A8), (object) a8),
          Tuple.NewTuple (typeof (A9), (object) a9)
          );
    }

    /// <summary>
    /// Creates a typed sequence with the given elements. The element types are specified by generic arguments, which can be automatically inferred
    /// by most compilers.
    /// </summary>
    /// <returns>A new <see cref="TypedSequence"/> instance holding the given values with their respective types.</returns>
    public static TypedSequence Create<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      return new TypedSequence (
          Tuple.NewTuple (typeof (A1), (object) a1), 
          Tuple.NewTuple (typeof (A2), (object) a2), 
          Tuple.NewTuple (typeof (A3), (object) a3), 
          Tuple.NewTuple (typeof (A4), (object) a4), 
          Tuple.NewTuple (typeof (A5), (object) a5), 
          Tuple.NewTuple (typeof (A6), (object) a6), 
          Tuple.NewTuple (typeof (A7), (object) a7), 
          Tuple.NewTuple (typeof (A8), (object) a8), 
          Tuple.NewTuple (typeof (A9), (object) a9), 
          Tuple.NewTuple (typeof (A10), (object) a10) 
          );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedSequence"/> class. For small number of values, use the <see cref="Create{A1}"/> overloads 
    /// instead.
    /// </summary>
    /// <param name="typedValues">The typed values to be held by this <see cref="TypedSequence"/>.</param>
    public TypedSequence (IEnumerable<Tuple<Type, object>> typedValues)
    {
      _typedValues = typedValues.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedSequence"/> class. For small number of values, use the <see cref="Create{A1}"/> overloads 
    /// instead.
    /// </summary>
    /// <param name="typedValues">The typed values to be held by this <see cref="TypedSequence"/>.</param>
    public TypedSequence (params Tuple<Type, object>[] typedValues)
        : this ((IEnumerable<Tuple<Type, object>>) typedValues)
    {
    }

    /// <summary>
    /// Gets the number of elements stored by this <see cref="TypedSequence"/>.
    /// </summary>
    /// <value>The element count.</value>
    public int Count
    {
      get { return _typedValues.Length; }
    }

    /// <summary>
    /// Gets an array holding the types of the elements stored by this <see cref="TypedSequence"/>.
    /// </summary>
    /// <value>The element types.</value>
    public Type[] Types
    {
      get { return Array.ConvertAll (_typedValues, tuple => tuple.A); }
    }

    /// <summary>
    /// Gets an array holding the element values stored by this <see cref="TypedSequence"/>.
    /// </summary>
    /// <value>The element values.</value>
    public object[] Values
    {
      get { return Array.ConvertAll (_typedValues, tuple => tuple.B); }
    }

    /// <summary>
    /// Returns an enumerator that iterates through this <see cref="TypedSequence"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the sequence.
    /// </returns>
    public IEnumerator<Tuple<Type, object>> GetEnumerator()
    {
      return ((IEnumerable<Tuple<Type, object>>) _typedValues).GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
