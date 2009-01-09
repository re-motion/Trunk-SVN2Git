// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Represents an untyped result of a collection query.
  /// </summary>
  /// <seealso cref="IQueryManager.GetCollection"/>
  /// <remarks>
  /// <para>
  /// The result might contain duplicates or <see langword="null"/> values when calling <see cref="AsEnumerable"/> or <see cref="ToArray"/>. To filter
  /// <see cref="IEnumerable{T}"/>
  /// them out, use the <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> and 
  /// <see cref="Enumerable.Where{TSource}(IEnumerable{TSource},Func{TSource,bool})"/> methods.
  /// </para>
  /// <para>
  /// <see cref="CollectionQueryResult{T}"/> implements this interface, but represents a typed result of a collection query.
  /// </para>
  /// </remarks>
  public interface ICollectionQueryResult
  {
    /// <summary>
    /// Gets the number of <see cref="DomainObject"/> instances returned by the query.
    /// </summary>
    /// <value>The result count.</value>
    int Count { get; }
    /// <summary>
    /// Gets the query used to construct this result.
    /// </summary>
    /// <value>The query that yielded the result.</value>
    IQuery Query { get; }

    /// <summary>
    /// Determines whether the result set contains duplicates.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if result set contains duplicates; otherwise, <c>false</c>.
    /// </returns>
    bool ContainsDuplicates ();
    /// <summary>
    /// Determines whether result set contains <see langword="null"/> values.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if result set contains <see langword="null"/> values; otherwise, <c>false</c>.
    /// </returns>
    bool ContainsNulls ();

    /// <summary>
    /// Returns the query result set as an enumerable object. Might contain duplicates or <see langword="null"/> values.
    /// </summary>
    /// <returns>An instance of <see cref="IEnumerable{T}"/> containing the <see cref="DomainObject"/> instances yielded by the query.</returns>
    IEnumerable<DomainObject> AsEnumerable ();
    /// <summary>
    /// Returns the query result set as an array. Might contain duplicates or <see langword="null"/> values.
    /// </summary>
    /// <returns>An array containing the <see cref="DomainObject"/> instances yielded by the query.</returns>
    DomainObject[] ToArray ();
    /// <summary>
    /// Returns the query result set as an <see cref="ObjectList{T}"/>. If the result set contains duplicates or <see langword="null"/> values, this
    /// method throws an exception.
    /// </summary>
    /// <returns>An instance of <see cref="ObjectList{T}"/> containing the <see cref="DomainObject"/> instances yielded by the query.</returns>
    /// <exception cref="InvalidOperationException">The query contains <see langword="null"/> values or duplicates.</exception>
    ObjectList<DomainObject> ToObjectList ();
  }
}