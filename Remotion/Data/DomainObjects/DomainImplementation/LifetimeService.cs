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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality to instantiate, get, and delete <see cref="DomainObject"/> instances.
  /// </summary>
  public static class LifetimeService
  {
    /// <summary>
    /// Returns a new instance of a <see cref="DomainObject"/> with the supplied constructor arguments in the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="domainObjectType">The <see cref="Type"/> of the <see cref="DomainObject"/> to be created.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> encapsulating the parameters to be passed to the constructor. Instantiate this
    /// by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// 	<para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in
    /// management tasks at runtime.
    /// </para>
    /// 	<para>
    /// This method should only be used by infrastructure code, ordinary code should use <see cref="DomainObject.NewObject{T}(ParamList)"/>.
    /// </para>
    /// 	<para>For more information, also see the constructor documentation (<see cref="DomainObject"/>).</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null" />.</exception>
    /// <exception cref="MappingException">The <paramref name="domainObjectType"/> parameter does not specify a domain object type with mapping information.</exception>
    /// <exception cref="ArgumentException">The type <paramref name="domainObjectType"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The <paramref name="domainObjectType"/> does not implement the required constructor (see Remarks
    /// section).
    /// </exception>
    public static DomainObject NewObject (ClientTransaction clientTransaction, Type domainObjectType, ParamList constructorParameters)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);

      return clientTransaction.NewObject (domainObjectType, constructorParameters);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that already exists or attempts to load it from the data source.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
    /// <returns>
    /// The <see cref="DomainObject"/> with the specified <paramref name="objectID"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> or <paramref name="objectID"/> are <see langword="null"/>.</exception>
    /// <exception cref="ObjectNotFoundException">The object could not be found in the data source.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    /// The Mapping does not contain a class definition for the given <paramref name="objectID"/>.<br/> -or- <br/>
    /// An error occurred while reading a <see cref="PropertyValue"/>.<br/> -or- <br/>
    /// An error occurred while accessing the datasource.
    /// </exception>
    /// <exception cref="ObjectDeletedException">The object has already been deleted and the <paramref name="includeDeleted"/> flag is 
    /// <see langword="false" />.</exception>
    public static DomainObject GetObject (ClientTransaction clientTransaction, ObjectID objectID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      // TODO: Check behavior with discarded objects.

      return clientTransaction.GetObject (objectID, includeDeleted);
    }

    /// <summary>
    /// Deletes the given <see cref="DomainObject"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectToBeDeleted">The object to be deleted.</param>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null" />.</exception>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the current transaction.</exception>
    /// <remarks>See also <see cref="DomainObject.Delete"/>.</remarks>
    public static void DeleteObject (ClientTransaction clientTransaction, DomainObject objectToBeDeleted)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectToBeDeleted", objectToBeDeleted);

      clientTransaction.Delete (objectToBeDeleted);
    }

    [Obsolete ("This method is now obsolete, use the overload taking a ClientTransaction instead. (1.13.42)", true)]
    public static DomainObject NewObject (Type domainObjectType, ParamList constructorParameters)
    {
      return NewObject (ClientTransaction.Current, domainObjectType, constructorParameters);
    }

    [Obsolete ("This method is now obsolete, use the overload taking a ClientTransaction instead. (1.13.42)", true)]
    public static DomainObject GetObject (ObjectID objectID, bool includeDeleted)
    {
      return GetObject (ClientTransactionScope.CurrentTransaction, objectID, includeDeleted);
    }

    [Obsolete ("This method is now obsolete, use the overload taking a ClientTransaction instead. (1.13.42)", true)]
    public static void DeleteObject (DomainObject objectToBeDeleted)
    {
      DeleteObject (objectToBeDeleted.DefaultTransactionContext.ClientTransaction, objectToBeDeleted);
    }
  }
}