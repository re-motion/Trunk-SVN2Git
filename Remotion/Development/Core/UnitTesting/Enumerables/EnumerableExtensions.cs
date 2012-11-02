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
using System.Collections.Generic;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Development.UnitTesting.Enumerables
{
  /// <summary>
  /// Provides extensions methods for <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Wraps an <see cref="IEnumerable{T}"/> to ensure that it is iterated only once.
    /// </summary>
    /// <typeparam name="T">The element type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IEnumerable{T}"/> to be wrapped.</param>
    /// <returns>An instance of <see cref="OneTimeEnumerable{T}"/> decorating the <paramref name="source"/>.</returns>
    public static OneTimeEnumerable<T> AsOneTime<T> (this IEnumerable<T> source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      return new OneTimeEnumerable<T> (source);
    }

    /// <summary>
    /// Forces the enumeration of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
    /// <returns>An array containing all values computed by <paramref name="source"/>.</returns>
    public static T[] ForceEnumeration<T> (this IEnumerable<T> source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      return source.ToArray();
    }
  }
}