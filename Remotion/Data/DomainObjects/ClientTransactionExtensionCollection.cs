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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// A collection of <see cref="IClientTransactionExtension"/>s.
  /// </summary>
  [Serializable]
  public class ClientTransactionExtensionCollection : CommonCollection, IClientTransactionExtension
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    /// <summary>
    /// Gets an <see cref="IClientTransactionExtension"/> by the extension name.
    /// </summary>
    /// <param name="extensionName">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <returns>The <see cref="IClientTransactionExtension"/> of the given <paramref name="extensionName"/> or <see langword="null"/> if the name was not found.</returns>
    public IClientTransactionExtension this[string extensionName]
    {
      get 
      {
        ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

        return (IClientTransactionExtension) BaseGetObject (extensionName); 
      }
    }

    /// <summary>
    /// Gets the <see cref="IClientTransactionExtension"/> of a given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the extension to be retrieved.</param>
    /// <returns>The <see cref="IClientTransactionExtension"/> of the given <paramref name="index"/>.</returns>
    public IClientTransactionExtension this[int index]
    {
      get { return (IClientTransactionExtension) BaseGetObject (index); }
    }

    /// <summary>
    /// Adds an <see cref="IClientTransactionExtension"/> to the collection.
    /// </summary>
    /// <param name="extensionName">A name for the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <param name="clientTransactionExtension">The extension to add. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentException">An extension with the given <paramref name="extensionName"/> is already part of the collection.</exception>
    /// <remarks>The order of the extensions in the collection is the order in which they are notified.</remarks>
    public void Add (string extensionName, IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);
      ArgumentUtility.CheckNotNull ("clientTransactionExtension", clientTransactionExtension);
      if (BaseContainsKey (extensionName)) 
        throw CreateArgumentException ("extensionName", "An extension with name '{0}' is already part of the collection.", extensionName);
      
      BaseAdd (extensionName, clientTransactionExtension);
    }

    /// <summary>
    /// Removes an <see cref="IClientTransactionExtension"/> from the collection.
    /// </summary>
    /// <param name="extensionName">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    public void Remove (string extensionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

      BaseRemove (extensionName);
    }

    /// <summary>
    /// Gets the index of an <see cref="IClientTransactionExtension"/> with a given <paramref name="extensionName"/>.
    /// </summary>
    /// <param name="extensionName">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <returns>The index of the extension, or -1 if <paramref name="extensionName"/> is not found.</returns>
    public int IndexOf (string extensionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

      return BaseIndexOfKey (extensionName);
    }

    /// <summary>
    /// Inserts an <see cref="IClientTransactionExtension"/> intto the collection at a specified index.
    /// </summary>
    /// <param name="extensionName">A name for the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <param name="clientTransactionExtension">The extension to insert. Must not be <see langword="null"/>.</param>
    /// <param name="index">The index where the extension should be inserted.</param>
    /// <exception cref="System.ArgumentException">An extension with the given <paramref name="extensionName"/> is already part of the collection.</exception>
    /// <remarks>The order of the extensions in the collection is the order in which they are notified.</remarks>
    public void Insert (int index, string extensionName, IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);
      ArgumentUtility.CheckNotNull ("clientTransactionExtension", clientTransactionExtension);
      if (BaseContainsKey (extensionName))
        throw CreateArgumentException ("extensionName", "An extension with name '{0}' is already part of the collection.", extensionName);

      BaseInsert (index, extensionName, clientTransactionExtension);
    }

    private ArgumentException CreateArgumentException (string parameterName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), parameterName);
    }

    #region Notification methods

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
      ArgumentUtility.CheckNotNull ("parentClientTransaction", parentClientTransaction);

      foreach (IClientTransactionExtension extension in this)
        extension.SubTransactionCreating (parentClientTransaction);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("parentClientTransaction", parentClientTransaction);
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);

      foreach (IClientTransactionExtension extension in this)
        extension.SubTransactionCreated (parentClientTransaction, subTransaction);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      foreach (IClientTransactionExtension extension in this)
        extension.NewObjectCreating (clientTransaction, type);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectsLoading (clientTransaction, objectIDs);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> loadedDomainObjects)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("loadedDomainObjects", loadedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectsLoaded (clientTransaction, loadedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectDeleting (clientTransaction, domainObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectDeleted (clientTransaction, domainObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueReading (clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueRead (clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueChanging (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueChanged (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationReading (clientTransaction, domainObject, propertyName, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationRead (clientTransaction, domainObject, propertyName, relatedObject, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ReadOnlyCollection<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationRead (clientTransaction, domainObject, propertyName, relatedObjects, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationChanging (clientTransaction, domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationChanged (clientTransaction, domainObject, propertyName);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      foreach (IClientTransactionExtension extension in this)
        queryResult = extension.FilterQueryResult (clientTransaction, queryResult);
      return queryResult;
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void Committing (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("changedDomainObjects", changedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.Committing (clientTransaction, changedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void Committed (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("changedDomainObjects", changedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.Committed (clientTransaction, changedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("changedDomainObjects", changedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.RollingBack (clientTransaction, changedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("changedDomainObjects", changedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.RolledBack (clientTransaction, changedDomainObjects);
    }

    #endregion
  }
}
