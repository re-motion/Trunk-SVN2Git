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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Provides a set of <see langword="static"/> methods for querying objects that implement <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Returns the first element of a sequence
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TException">Type type of the exception returned by <paramref name="createEmptySequenceException"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to return the first element of. Must not be <see langword="null" />.</param>
    /// <param name="createEmptySequenceException">
    /// This callback is invoked if the sequence is empty. The returned exception is then thrown to indicate this error. Must not be <see langword="null" />.
    /// </param>
    /// <returns>The first element in the specified sequence.</returns>
    public static TSource First<TSource, TException> (this IEnumerable<TSource> source, Func<TException> createEmptySequenceException)
        where TException: Exception
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("createEmptySequenceException", createEmptySequenceException);

      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current;
      }

      throw createEmptySequenceException();
    }

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TException">Type type of the exception returned by <paramref name="createNoMatchingElementException"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to return an element of. Must not be <see langword="null" />.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="createNoMatchingElementException">
    /// This callback is invoked if the sequence is empty or no element satisfies the condition in <paramref name="predicate"/>. 
    /// The returned exception is then thrown to indicate this error. Must not be <see langword="null" />.
    /// </param>
    /// <returns>The first element in the sequence that passes the test in the specified predicate function.</returns>
    public static TSource First<TSource, TException> (
        this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TException> createNoMatchingElementException)
        where TException: Exception
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("predicate", predicate);
      ArgumentUtility.CheckNotNull ("createNoMatchingElementException", createNoMatchingElementException);

      foreach (TSource current in source)
      {
        if (predicate (current))
          return current;
      }

      throw createNoMatchingElementException();
    }

    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TException">Type type of the exception returned by <paramref name="createEmptySequenceException"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to return the single element of. Must not be <see langword="null" />.</param>
    /// <param name="createEmptySequenceException">
    /// This callback is invoked if the sequence is empty. 
    /// The returned exception is then thrown to indicate this error. Must not be <see langword="null" />.
    /// </param>
    /// <returns>The single element in the specified sequence.</returns>
    /// <exception cref="InvalidOperationException">InvalidOperationException The specified sequence contains more than one element.</exception>
    public static TSource Single<TSource, TException> (this IEnumerable<TSource> source, Func<TException> createEmptySequenceException)
        where TException: Exception
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("createNoMatchingElementException", createEmptySequenceException);

      TSource result = default (TSource);
      bool isElementFound = false;
      foreach (TSource current in source)
      {
        if (isElementFound)
          throw new InvalidOperationException ("Sequence contains more than one element.");

        isElementFound = true;
        result = current;
      }

      if (isElementFound)
        return result;

      throw createEmptySequenceException();
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TException">Type type of the exception returned by <paramref name="createNoMatchingElementException"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to return a single element of. Must not be <see langword="null" />.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="createNoMatchingElementException">
    /// This callback is invoked if the sequence is empty or no element satisfies the condition in <paramref name="predicate"/>. 
    /// The returned exception is then thrown to indicate this error. Must not be <see langword="null" />.
    /// </param>
    /// <returns>The single element in the specified sequence.</returns>
    /// <exception cref="InvalidOperationException">InvalidOperationException The specified sequence contains more than one element.</exception>
    public static TSource Single<TSource, TException> (
        this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TException> createNoMatchingElementException)
        where TException: Exception
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("predicate", predicate);
      ArgumentUtility.CheckNotNull ("createNoMatchingElementException", createNoMatchingElementException);

      TSource result = default (TSource);
      bool isElementFound = false;
      foreach (TSource current in source)
      {
        if (predicate (current))
        {
          if (isElementFound)
            throw new InvalidOperationException ("Sequence contains more than one matching element.");

          isElementFound = true;
          result = current;
        }
      }

      if (isElementFound)
        return result;

      throw createNoMatchingElementException();
    }
  }
}