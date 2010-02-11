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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
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

    [NonSerialized]
    private RootQueryManager _queryManager;
    private readonly Guid _id = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <b>RootClientTransaction</b> class.
    /// </summary>
    protected RootClientTransaction ()
      : base (
        new Dictionary<Enum, object>(), 
        new ClientTransactionExtensionCollection (), 
        new RootCollectionEndPointChangeDetectionStrategy(),
        new DictionaryBasedEnlistedDomainObjectManager())
    {
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
          _queryManager = new RootQueryManager (this, ObjectLoader);

        return _queryManager;
      }
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

    protected internal override DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (var persistenceManager = new PersistenceManager (_id))
      {
        return persistenceManager.LoadDataContainer (id);
      }
    }

    protected internal override DataContainerCollection LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Count == 0)
        return new DataContainerCollection();

      foreach (var id in objectIDs)
      {
        if (DataManager.IsDiscarded (id))
          throw new ObjectDiscardedException (id);
      }

      using (var persistenceManager = new PersistenceManager (_id))
      {
        return persistenceManager.LoadDataContainers (objectIDs, throwOnNotFound);
      }
    }

    protected internal override DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
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
        return relatedDataContainer;
      }
    }

    protected internal override DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      using (var persistenceManager = new PersistenceManager (_id))
      {
        return persistenceManager.LoadRelatedDataContainers (relationEndPointID);
      }
    }

    protected override DataContainer[] LoadDataContainersForQuery (IQuery query)
    {
      using (var storageProviderManager = new StorageProviderManager (ID))
      {
        StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
        return provider.ExecuteCollectionQuery (query);
      }
    }
  }
}
