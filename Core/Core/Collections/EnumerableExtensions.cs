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
    /// <param name="predicate">A function to test each element for a condition. Must not be <see langword="null" />.</param>
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
    /// <param name="predicate">A function to test each element for a condition. Must not be <see langword="null" />.</param>
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

    /// <summary>
    /// Generates a sequence of elements from the <paramref name="source"/> element by applying the specified next-element function, 
    /// adding elements to the sequence while the current element satisfies the specified condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the <paramref name="source"/> element.</typeparam>
    /// <param name="source">The object to be transformed into a sequence.</param>
    /// <param name="nextElementSelector">A function to retrieve the next element in the sequence. Must not be <see langword="null" />.</param>
    /// <param name="predicate">A function to test each element for a condition. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A collection of elements containing the <paramref name="source"/> and all subsequent elements where each element satisfies a specified condition.
    /// </returns>
    public static IEnumerable<TSource> CreateSequence<TSource> (this TSource source, Func<TSource, TSource> nextElementSelector, Func<TSource, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("nextElementSelector", nextElementSelector);
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      for (TSource current = source; predicate (current); current = nextElementSelector (current))
        yield return current;
    }

    /// <summary>
    /// Generates a sequence of elements from the <paramref name="source"/> element by applying the specified next-element function, 
    /// adding elements to the sequence while the current element is not <see langword="null" />.
    /// </summary>
    /// <typeparam name="TSource">The type of the <paramref name="source"/> element.</typeparam>
    /// <param name="source">The object to be transformed into a sequence.</param>
    /// <param name="nextElementSelector">A function to retrieve the next element in the sequence. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A sequence of elements containing the <paramref name="source"/> and all subsequent elements where each element satisfies a specified condition.
    /// </returns>
    public static IEnumerable<TSource> CreateSequence<TSource> (this TSource source, Func<TSource, TSource> nextElementSelector)
        where TSource : class
    {
      ArgumentUtility.CheckNotNull ("nextElementSelector", nextElementSelector);

      return CreateSequence (source, nextElementSelector, e => e != null);
    }

    /// <summary>
    /// Creates an <see cref="IEnumerable{T}"/> containing the <paramref name="source"/> element.
    /// </summary>
    /// <typeparam name="TSource">The type of the <paramref name="source"/> element.</typeparam>
    /// <param name="source">The object to be added to the sequence. </param>
    /// <returns>A sequence of elments containing only the <paramref name="source"/> element.</returns>
    public static IEnumerable<TSource> ToEnumerable<TSource> (this TSource source)
    {
      yield return source;
    }
  }
}