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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Reflection;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents a top-level <see cref="ClientTransaction"/>, which does not have a parent transaction.
  /// </summary>
  [Serializable]
  public class RootClientTransaction : ClientTransaction
  {
    /// <summary>
    /// Do not use this method, use <see>ClientTransaction.CreateBindingTransaction</see> instead.
    /// </summary>
    /// <returns></returns>
    [Obsolete ("Use ClientTransaction.CreateBindingTransaction for clarity.")]
    public static new ClientTransaction CreateBindingTransaction ()
    {
      return ClientTransaction.CreateBindingTransaction ();
    }

    private readonly Dictionary<ObjectID, DomainObject> _enlistedObjects;
    [NonSerialized]
    private RootQueryManager _queryManager;
    private readonly Guid _id = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <b>RootClientTransaction</b> class.
    /// </summary>
    protected RootClientTransaction ()
      : base (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection (), new RootCollectionEndPointChangeDetectionStrategy())
    {
      _enlistedObjects = new Dictionary<ObjectID, DomainObject>();
    }

    public override ClientTransaction ParentTransaction
    {
      get { return null; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return this; }
    }

    public Guid ID
    {
      get { return _id; }
    }

    /// <summary>Initializes a new instance of this transaction.</summary>
    public override ClientTransaction CreateEmptyTransactionOfSameType ()
    {
      return (ClientTransaction) TypesafeActivator.CreateInstance (GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With ();
    }

    public override IQueryManager QueryManager
    {
      get
      {
        if (_queryManager == null)
          _queryManager = new RootQueryManager (this);

        return _queryManager;
      }
    }

    protected internal override bool DoEnlistDomainObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      DomainObject alreadyEnlistedObject = GetEnlistedDomainObject (domainObject.ID);
      if (alreadyEnlistedObject != null && alreadyEnlistedObject != domainObject)
      {
        string message = string.Format ("A domain object instance for object '{0}' already exists in this transaction.", domainObject.ID);
        throw new InvalidOperationException (message);
      }
      else if (alreadyEnlistedObject == null)
      {
        _enlistedObjects.Add (domainObject.ID, domainObject);
        return true;
      }
      else
        return false;
    }

    protected internal override bool IsEnlisted (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      return GetEnlistedDomainObject (domainObject.ID) == domainObject;
    }

    protected internal override DomainObject GetEnlistedDomainObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      DomainObject domainObject;
      _enlistedObjects.TryGetValue (objectID, out domainObject);
      return domainObject;
    }

    protected internal override IEnumerable<DomainObject> EnlistedDomainObjects
    {
      get { return _enlistedObjects.Values; }
    }

    protected internal override int EnlistedDomainObjectCount
    {
      get { return _enlistedObjects.Count; }
    }

    protected override void PersistData (DataContainerCollection changedDataContainers)
    {
      ArgumentUtility.CheckNotNull ("changedDataContainers", changedDataContainers);

      if (changedDataContainers.Count > 0)
      {
        using (var persistenceManager = new PersistenceManager (_id))
        {
          persistenceManager.Save (changedDataContainers);
        }
      }
    }

    protected internal override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ObjectID newObjectID;
      using (var persistenceManager = new PersistenceManager (_id))
      {
        newObjectID = persistenceManager.CreateNewObjectID (classDefinition);
      }
      return newObjectID;
    }

    protected override DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (var persistenceManager = new PersistenceManager (_id))
      {
        var dataContainer = persistenceManager.LoadDataContainer (id);
        TransactionEventSink.ObjectLoading (dataContainer.ID);
        return dataContainer;
      }
    }

    protected override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      foreach (ObjectID id in objectIDs)
      {
        if (DataManager.IsDiscarded (id))
          throw new ObjectDiscardedException (id);
      }

      using (var persistenceManager = new PersistenceManager (_id))
      {
        var newLoadedDataContainers = persistenceManager.LoadDataContainers (objectIDs, throwOnNotFound);
        foreach (DataContainer dataContainer in newLoadedDataContainers)
          TransactionEventSink.ObjectLoading (dataContainer.ID);
        return newLoadedDataContainers;
      }
    }

    protected internal override DataContainer LoadDataContainerForExistingObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      using (EnterNonDiscardingScope ())
      {
        // ensure that the transaction knows the given object, that way, LoadDataContainer will associate the new DataContainer with it
        EnlistDomainObject (domainObject);
        return LoadDataContainer (domainObject.ID);
      }
    }

    protected override DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, false);
      DataContainer relatedDataContainer;
      using (var persistenceManager = new PersistenceManager (_id))
      {
        relatedDataContainer = persistenceManager.LoadRelatedDataContainer (GetDataContainer (domainObject), relationEndPointID);
        
        // This assertion is only true if single related objects are never loaded lazily; otherwise, a "merge" would be necessary.
        // (Like in MergeLoadedDomainObjects.)
        Assertion.IsTrue (relatedDataContainer == null || DataManager.DataContainerMap[relatedDataContainer.ID] == null, 
            "ObjectEndPoints are created eagerly, so this related object can't have been loaded so far. "
            + "(Otherwise LoadRelatedDataContainer wouldn't have been called.)");
        if (relatedDataContainer != null)
          TransactionEventSink.ObjectLoading (relatedDataContainer.ID);
      }
      return relatedDataContainer;
    }

    protected override DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      DataContainerCollection relatedDataContainers;
      using (var persistenceManager = new PersistenceManager (_id))
      {
        relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);
      }
      return relatedDataContainers;
    }
  }
}
