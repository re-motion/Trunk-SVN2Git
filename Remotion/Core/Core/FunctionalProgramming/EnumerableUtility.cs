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
using System.Collections;
using System.Collections.Generic;

namespace Remotion.FunctionalProgramming
{
  /// <summary>
  /// Provides helper functions for <see cref="IEnumerable{T}"/> objects.
  /// </summary>
  /// <remarks>
  /// Most of these methods will become obsolete with C# 3.0/LINQ.
  /// </remarks>
  public static class EnumerableUtility
  {
    /// <summary>
    /// Combines the specified <see cref="IEnumerable{T}"/> sequences into a single sequence.
    /// </summary>
    /// <typeparam name="T">The item type of the sequences to combine.</typeparam>
    /// <param name="sources">The source sequences to combine.</param>
    /// <returns>A single sequence yielding the items of each source sequence in the same order in which they were passed via the 
    /// <paramref name="sources"/> parameter.</returns>
    public static IEnumerable<T> Combine<T> (params IEnumerable<T>[] sources)
    {
      for (int i = 0; i < sources.Length; ++i)
      {
        foreach (T item in sources[i])
          yield return item;
      }
    }

    [Obsolete ("This method is obsolete with LINQ, use source.Select (selector) instead. (1.13.42)", true)]
    public static IEnumerable<TResult> Select<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use source.Select (selector).ToArray() instead. (1.13.42)", true)]
    public static TResult[] SelectToArray<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use source.Where (predicate) instead. (1.13.42)", true)]
    public static IEnumerable<TSource> Where<TSource> (IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use Combine (sources).ToArray() instead. (1.13.42)", true)]
    public static T[] CombineToArray<T> (params IEnumerable<T>[] sources)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use source.ToList() instead. (1.13.42)", true)]
    public static IList<T> ToList<T> (IEnumerable<T> source)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use source.ToList() instead. (1.13.42)", true)]
    public static T[] ToArray<T> (IEnumerable<T> source)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use sourceEnumerable.Cast<T>() instead. (1.13.42)", true)]
    public static IEnumerable<T> Cast<T> (IEnumerable sourceEnumerable)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method is obsolete with LINQ, use source.FirstOrDefault() instead. (1.13.42)", true)]
    public static T FirstOrDefault<T> (IEnumerable<T> source)
    {
      throw new NotImplementedException ();
    }

    /// <summary>
    /// Creates an <see cref="IEnumerable{T}"/> containing <paramref name="item"/> as its single element.
    /// </summary>
    /// <typeparam name="TItem">The type of the <paramref name="item"/> element.</typeparam>
    /// <param name="item">The object to be added to the sequence. Can be <see langword="null" />.</param>
    /// <returns>A sequence containing only the <paramref name="item"/> element.</returns>
    public static IEnumerable<TItem> Singleton<TItem> (TItem item)
    {
      yield return item;
    }
  }
}