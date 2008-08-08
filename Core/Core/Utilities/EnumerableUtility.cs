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
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides helper functions for <see cref="IEnumerable{T}"/> objects.
  /// </summary>
  /// <remarks>
  /// Most of these methods will become obsolete with C# 3.0/LINQ.
  /// </remarks>
  public static class EnumerableUtility
  {
    public static IEnumerable<TResult> Select<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      foreach (TSource item in source)
        yield return selector (item);
    }

    public static TResult[] SelectToArray<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      if (source == null)
        return null;

      IList<TSource> sourceList = ToList (source);

      TResult[] result = new TResult[sourceList.Count];
      for (int i = 0; i < sourceList.Count; ++i)
        result[i] = selector (sourceList[i]);

      return result;
    }

    public static IEnumerable<TSource> Where<TSource> (IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      foreach (TSource item in source)
      {
        if (predicate (item))
          yield return item;
      }
    }

    public static IEnumerable<T> Combine<T> (params IEnumerable<T>[] sources)
    {
      for (int i = 0; i < sources.Length; ++i)
      {
        foreach (T item in sources[i])
          yield return item;
      }
    }

    public static T[] CombineToArray<T> (params IEnumerable<T>[] sources)
    {
      return ToArray (Combine (sources));
    }
    
    public static IList<T> ToList<T> (IEnumerable<T> source)
    {
      IList<T> list = source as IList<T>;
      if (list != null)
        return list;
      else
        return new List<T> (source);
    }

    public static T[] ToArray<T> (IEnumerable<T> source)
    {
      T[] array = source as T[];
      if (array != null)
        return array;
      else
        return new List<T> (source).ToArray();
    }

    public static IEnumerable<T> Cast<T> (IEnumerable sourceEnumerable)
    {
      ArgumentUtility.CheckNotNull ("sourceEnumerable", sourceEnumerable);
      foreach (object o in sourceEnumerable)
        yield return (T) o;
    }

    public static T FirstOrDefault<T> (IEnumerable<T> source)
    {
      using (IEnumerator<T> enumerator = source.GetEnumerator())
      {
        if (enumerator.MoveNext ())
          return enumerator.Current;
        else
          return default (T);
      }
    }
  }
}
