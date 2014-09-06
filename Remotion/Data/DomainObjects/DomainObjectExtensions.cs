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

using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides useful extension methods for working with <see cref="DomainObject"/> instances.
  /// </summary>
  public static class DomainObjectExtensions
  {
    /// <summary>
    /// Returns the <see cref="DomainObject.ID"/> of the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if 
    /// <paramref name="domainObjectOrNull"/> is itself <see langword="null" />.
    /// </summary>
    /// <param name="domainObjectOrNull">The <see cref="IDomainObject"/> whose <see cref="IDomainObject.ID"/> to get. If this parameter is 
    /// <see langword="null" />, the method returns <see langword="null" />.</param>
    /// <returns>The <paramref name="domainObjectOrNull"/>'s <see cref="DomainObject.ID"/>, or <see langword="null" /> if <paramref name="domainObjectOrNull"/>
    /// is <see langword="null" />.</returns>
    [CanBeNull]
    [ContractAnnotation ("null => null; notnull => notnull")]
    public static ObjectID GetSafeID ([CanBeNull] this IDomainObject domainObjectOrNull)
    {
      if (domainObjectOrNull == null)
        return null;

      var objectID = domainObjectOrNull.ID;
      Assertion.DebugIsNotNull (objectID, "domainObjectOrNull.ID was null");

      return domainObjectOrNull.ID;
    }

    /// <summary>
    /// Returns a typed handle to the given <paramref name="domainObject"/>. The generic type parameter <typeparamref name="T"/> is inferred from the 
    /// static type of the value passed as <paramref name="domainObject"/>.
    /// </summary>
    /// <typeparam name="T">The type to be used for the returned <see cref="IDomainObjectHandle{T}"/>.</typeparam>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to get a handle for. Must not be <see langword="null" />.</param>
    /// <returns>A typed handle to the given <paramref name="domainObject"/>.</returns>
    [NotNull]
    public static IDomainObjectHandle<T> GetHandle<T> ([NotNull] this T domainObject) where T : IDomainObject
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      var objectID = domainObject.ID;
      Assertion.DebugIsNotNull (objectID, "domainObject.ID was null");

      return objectID.GetHandle<T>();
    }

    /// <summary>
    /// Returns a typed handle to the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if 
    /// <paramref name="domainObjectOrNull"/> is itself <see langword="null" />.
    /// The generic type parameter <typeparamref name="T"/> is inferred from the 
    /// static type of the value passed as <paramref name="domainObjectOrNull"/>.
    /// </summary>
    /// <typeparam name="T">The type to be used for the returned <see cref="IDomainObjectHandle{T}"/>.</typeparam>
    /// <param name="domainObjectOrNull">The <see cref="IDomainObject"/> to get a handle for. If this parameter is 
    /// <see langword="null" />, the method returns <see langword="null" />.</param>
    /// <returns>A typed handle to the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if <paramref name="domainObjectOrNull"/>
    /// is <see langword="null" />.</returns>
    [CanBeNull]
    [ContractAnnotation ("null => null; notnull => notnull")]
    public static IDomainObjectHandle<T> GetSafeHandle<T> ([CanBeNull] this T domainObjectOrNull) where T : class, IDomainObject
    {
      return domainObjectOrNull != null ? domainObjectOrNull.GetHandle() : null;
    }

    /// <summary>
    /// Gets the current state of the <paramref name="domainObject"/> in the <see cref="ClientTransaction.ActiveTransaction"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to get the <see cref="StateType"/> for. Must not be <see langword="null" />.</param>
    public static StateType GetState ([NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull ("domainObject", domainObject);

      return GetDefaultTransactionContext (domainObject).State;
    }

    /// <summary>
    /// Gets the default <see cref="IDomainObjectTransactionContext"/>, i.e. the transaction context that is used when this 
    /// <see cref="IDomainObject"/>'s properties are accessed without specifying a <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <param name="domainObject">
    /// The <see cref="IDomainObject"/> to get the default <see cref="IDomainObjectTransactionContext"/> for. Must not be <see langword="null" />.
    /// </param>
    /// <returns>The default transaction context.</returns>
    /// <remarks>
    /// The default transaction for a <see cref="DomainObject"/> is the <see cref="ClientTransaction.ActiveTransaction"/> of the associated 
    /// <see cref="IDomainObject.RootTransaction"/>. The <see cref="ClientTransaction.ActiveTransaction"/> is usually the 
    /// <see cref="ClientTransaction.LeafTransaction"/>, but it can be changed by using <see cref="ClientTransaction"/> APIs.
    /// </remarks>
    [NotNull]
    public static IDomainObjectTransactionContext GetDefaultTransactionContext ([NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull ("domainObject", domainObject);

      var rootTransaction = domainObject.RootTransaction;
      Assertion.DebugAssert (rootTransaction != null, "domainObject.RootTransaction was null");

      return domainObject.TransactionContext[rootTransaction.ActiveTransaction];
    }
  }
}