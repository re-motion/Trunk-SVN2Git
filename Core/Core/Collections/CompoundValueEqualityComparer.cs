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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// <para>Class supplying automatic consistent <see cref="Equals"/> and <see cref="GetHashCode"/> implementation
  /// for instances of its generic type parameter, using a <see cref="Func{T,TResult}"/> which returns
  /// the instance members which shall participate in the equality/hash code calculation.
  /// </para>
  /// <para>
  /// Note: The performance of the implementation incurs the overhead of the creation of the <see cref="object"/>-array every time
  /// <see cref="Equals"/> and <see cref="GetHashCode"/> get called. Be aware of this when using in performance critical code.
  /// </para>
  /// </summary>
  /// <typeparam name="T">Type for which <see cref="Equals"/> and <see cref="GetHashCode"/> are supplied.</typeparam>
  public class CompoundValueEqualityComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, object[]> _equalityParticipantsProvider;

    /// <summary>
    /// Ctor which takes the <see cref="Func{T,TResult}"/> which must return an <see cref="object"/>-array
    /// of the members which shall participate in the equality/hash code calculation.
    /// </summary>
    /// <param name="relevantValueProvider"></param>
    public CompoundValueEqualityComparer (Func<T, object[]> relevantValueProvider)
    {
      _equalityParticipantsProvider = relevantValueProvider;
    }

    /// <summary>
    /// <see cref="Equals"/> implementation comparing all <see cref="object"/>|s in the array returned by the <see cref="Func{T,TResult}"/>.
    /// </summary>
    public bool Equals (T x, T y)
    {
      var equalityParticipantsX = _equalityParticipantsProvider (x);
      var equalityParticipantsY = _equalityParticipantsProvider (y);
      return equalityParticipantsX.SequenceEqual (equalityParticipantsY);
    }

    /// <summary>
    /// <see cref="GetHashCode"/> implementation using all the <see cref="object"/>|s in the array returned by the <see cref="Func{T,TResult}"/>.
    /// </summary>
    /// <remarks>
    /// Returned hash code uses the <see cref="EqualityUtility.GetRotatedHashCode(object[])"/> method.
    /// </remarks>
    public int GetHashCode (T x)
    {
      var equalityParticipantsX = _equalityParticipantsProvider (x);
      return EqualityUtility.GetRotatedHashCode (equalityParticipantsX);
    }

    /// <summary>
    /// Returns the <see cref="object"/>-array of the objects participating in the equality/hash code calculation for the passed instance.
    /// </summary>
    public ReadOnlyCollection<object> GetEqualityParticipatingObjects (T x)
    {
      return new ReadOnlyCollection<object>(_equalityParticipantsProvider (x));
    }
  }
}
