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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Reflection;
using Remotion.Utilities;

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

    /// <summary>
    /// Initializes a new instance of the <b>RootClientTransaction</b> class.
    /// </summary>
    public RootClientTransaction ()
      : base (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection ())
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

    /// <summary>Initializes a new instance of this transaction.</summary>
    public override ClientTransaction CreateEmptyTransactionOfSameType ()
    {
      return (ClientTransaction) TypesafeActivator.CreateInstance (GetType()).With();
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
        using (PersistenceManager persistenceManager = new PersistenceManager())
        {
          persistenceManager.Save (changedDataContainers);
        }
      }
    }

    protected internal override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ObjectID newObjectID;
      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        newObjectID = persistenceManager.CreateNewObjectID (classDefinition);
      }
      return newObjectID;
    }

    protected override DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        DataContainer dataContainer = persistenceManager.LoadDataContainer (id);
        TransactionEventSink.ObjectLoading (dataContainer.ID);
        SetClientTransaction (dataContainer);

        DataManager.RegisterExistingDataContainer (dataContainer);
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

      using (PersistenceManager persistenceManager = new PersistenceManager ())
      {
        DataContainerCollection newLoadedDataContainers = persistenceManager.LoadDataContainers (objectIDs, throwOnNotFound);
        NotifyOfLoading (newLoadedDataContainers);
        SetClientTransaction (newLoadedDataContainers);
        
        DataManager.RegisterExistingDataContainers (newLoadedDataContainers);
        return newLoadedDataContainers;
      }
    }

    protected internal override DataContainer LoadDataContainerForExistingObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      using (EnterNonDiscardingScope ())
      {
        DataContainer dataContainer = LoadDataContainer (domainObject.ID);
        dataContainer.SetDomainObject (domainObject);

        return dataContainer;
      }
    }

    protected internal override DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      using (EnterNonDiscardingScope())
      {
        DomainObject domainObject = GetObject (relationEndPointID.ObjectID, false);

        using (PersistenceManager persistenceManager = new PersistenceManager())
        {
          DataContainer relatedDataContainer = persistenceManager.LoadRelatedDataContainer (domainObject.GetDataContainerForTransaction(this), relationEndPointID);
          if (relatedDataContainer != null)
          {
            TransactionEventSink.ObjectLoading (relatedDataContainer.ID);
            SetClientTransaction (relatedDataContainer);
            DataManager.RegisterExistingDataContainer (relatedDataContainer);

            DomainObjectCollection loadedDomainObjects = new DomainObjectCollection (new DomainObject[] {relatedDataContainer.DomainObject}, true);
            OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

            return relatedDataContainer.DomainObject;
          }
          else
          {
            DataManager.RelationEndPointMap.RegisterObjectEndPoint (relationEndPointID, null);
            return null;
          }
        }
      }
    }

    protected internal override DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      using (EnterNonDiscardingScope())
      {
        using (PersistenceManager persistenceManager = new PersistenceManager())
        {
          DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);
          return MergeLoadedDomainObjects (relatedDataContainers, relationEndPointID);
        }
      }
    }

    protected internal override bool HasCollectionEndPointDataChanged (DomainObjectCollection currentData, DomainObjectCollection originalData)
    {
      return !DomainObjectCollection.Compare (currentData, originalData, true);
    }
  }
}
