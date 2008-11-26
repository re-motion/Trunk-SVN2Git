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
using Remotion.Data.DomainObjects;
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
    /// Returns an invocation object for creating a new instance of a concrete domain object. The object is created in the 
    /// <see cref="DomainObjects.ClientTransaction"/> that is active at constructor invocation time (i.e. when the invocation object is executed).
    /// </summary>
    /// <param name="domainObjectType">The <see cref="Type"/> of the <see cref="DomainObject"/> to be created.</param>
    /// <returns>An <see cref="IFuncInvoker{T}"/> object used to create a new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// This method's return value is an <see cref="IFuncInvoker{T}"/> object, which can be used to specify the required constructor and 
    /// pass it the necessary arguments in order to create a new domain object. Depending on the mapping being used by the object, one of two
    /// methods of object creation is used: legacy or via factory.
    /// </para>
    /// <para>
    /// Legacy objects are created by simply invoking the constructor matching the arguments passed to the <see cref="FuncInvoker{T}"/>
    /// object returned by this method.
    /// </para>
    /// <para>
    /// Objects created by the factory are not directly instantiated; instead a proxy is dynamically created for performing management tasks.
    /// </para>
    /// <para>This method should only be used by infrastructure code, ordinary code should use <see cref="DomainObject.NewObject{T}"/>.</para>
    /// <para>For more information, also see the constructor documentation (<see cref="DomainObject"/>).</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObjectType"/> parameter is null.</exception>
    /// <exception cref="MappingException">The <paramref name="domainObjectType"/> parameter does not specify a domain object type with mapping information.</exception>
    /// <exception cref="ArgumentException">The type <paramref name="domainObjectType"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The <paramref name="domainObjectType"/> does not implement the required constructor (see Remarks
    /// section).
    /// </exception>
    public static IFuncInvoker<DomainObject> NewObject (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      return GetCreator (domainObjectType).GetTypesafeConstructorInvoker (domainObjectType);
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
    /// Deletes the given <see cref="DomainObject"/> in the object's <see cref="DomainObject.ClientTransaction"/>.
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

    internal static IDomainObjectCreator GetCreator (Type domainObjectType)
    {
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType).GetDomainObjectCreator ();
    }

    /// <summary>
    /// Creates a <see cref="DomainObject"/> from a given data container.
    /// </summary>
    /// <param name="dataContainer">The data container for the new domain object.</param>
    /// <returns>A new <see cref="DomainObject"/> for the given data container.</returns>
    /// <remarks>
    /// <para>This method is used by the <see cref="DataContainer"/> class when it is asked to load an object. It requires an infrastructure
    /// constructor taking a single <see cref="DataContainer"/> argument on the domain object's class (see <see cref="DomainObject"/>).
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="dataContainer"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="MissingMethodException">The instantiated type does not implement the required public or protected constructor
    /// (see Remarks section).</exception>
    /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller.</exception>
    internal static DomainObject NewObjectFromDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      return RepositoryAccessor.GetCreator (dataContainer.DomainObjectType).CreateWithDataContainer (dataContainer);
    }
  }
}
