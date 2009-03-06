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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides functionality for infrastructure code to instantiate, get, and delete <see cref="DomainObject"/> instances
  /// without going through the usual methods defined by <see cref="DomainObject"/>. It also supports creating objects given
  /// a <see cref="Type"/> object.
  /// </summary>
  public static class RepositoryAccessor
  {
    /// <summary>
    /// Returns a new instance of a concrete domain object. The object is created with the supplied constructor arguments in the 
    /// <see cref="DomainObjects.ClientTransaction.Current"/> <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <param name="domainObjectType">The <see cref="Type"/> of the <see cref="DomainObject"/> to be created.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> encapsulating the parameters to be passed to the constructor. Instantiate this
    /// by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in 
    /// management tasks at runtime.
    /// </para>
    /// <para>
    /// This method should only be used by infrastructure code, ordinary code should use <see cref="DomainObject.NewObject{T}(ParamList)"/>.
    /// </para>
    /// <para>For more information, also see the constructor documentation (<see cref="DomainObject"/>).</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObjectType"/> parameter is null.</exception>
    /// <exception cref="MappingException">The <paramref name="domainObjectType"/> parameter does not specify a domain object type with mapping information.</exception>
    /// <exception cref="ArgumentException">The type <paramref name="domainObjectType"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The <paramref name="domainObjectType"/> does not implement the required constructor (see Remarks
    /// section).
    /// </exception>
    public static DomainObject NewObject (Type domainObjectType, ParamList constructorParameters)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);

      var creator = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType).GetDomainObjectCreator ();
      var ctorInfo = creator.GetConstructorLookupInfo (domainObjectType);
      return (DomainObject) constructorParameters.InvokeConstructor (ctorInfo);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
    /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="objectID"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="objectID"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="objectID"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the datasource.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    public static DomainObject GetObject (ObjectID objectID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return ClientTransactionScope.CurrentTransaction.GetObject (objectID, includeDeleted);
    }

    /// <summary>
    /// Deletes the given <see cref="DomainObject"/> in the default transaction, ie. the object's binding transaction or 
    /// - if none - <see cref="ClientTransaction.Current"/>.
    /// </summary>
    /// <param name="objectToBeDeleted">The object to be deleted.</param>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the current transaction.</exception>
    /// <remarks>See also <see cref="DomainObject.Delete"/>.</remarks>
    public static void DeleteObject (DomainObject objectToBeDeleted)
    {
      ArgumentUtility.CheckNotNull ("objectToBeDeleted", objectToBeDeleted);

      ClientTransaction transaction = DomainObjectCheckUtility.GetNonNullClientTransaction(objectToBeDeleted);

      DomainObjectCheckUtility.CheckIfObjectIsDiscarded (objectToBeDeleted, transaction);
      DomainObjectCheckUtility.CheckIfRightTransaction (objectToBeDeleted, transaction);
      transaction.Delete (objectToBeDeleted);
    }
  }
}
