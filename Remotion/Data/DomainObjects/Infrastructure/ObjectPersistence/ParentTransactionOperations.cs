﻿using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction operations required by <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public class ParentTransactionOperations : IParentTransactionOperations
  {
    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;
    private readonly IDisposable _scope;

    private bool _disposed = false;

    public ParentTransactionOperations (
        ClientTransaction parentTransaction,
        IInvalidDomainObjectManager parentInvalidDomainObjectManager,
        IDisposable scope)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("scope", scope);

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;

      _scope = scope;
    }

    public void Dispose ()
    {
      if (!_disposed)
      {
        _disposed = true;
        _scope.Dispose ();
      }
    }

    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckDisposed ();
      
      return _parentTransaction.CreateNewObjectID (classDefinition);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();
      
      return _parentTransaction.GetObject (objectID, false);
    }

    public IEnumerable<DomainObject> GetObjects (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
      CheckDisposed ();
      
      return _parentTransaction.GetObjects<DomainObject> (objectIDs, throwOnNotFound);
    }

    public DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      CheckDisposed ();
      
      return _parentTransaction.GetRelatedObject (relationEndPointID);
    }

    public IEnumerable<DomainObject> GetRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      CheckDisposed ();
      
      return _parentTransaction.GetRelatedObjects (relationEndPointID).Cast<DomainObject> ();
    }

    public QueryResult<DomainObject> ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      CheckDisposed ();
      
      return _parentTransaction.QueryManager.GetCollection (query);
    }

    public object ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      CheckDisposed ();
      
      return _parentTransaction.QueryManager.GetScalar (query);
    }

    public DataContainer GetDataContainerWithoutLoading (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();

      return _parentTransaction.DataManager.GetDataContainerWithoutLoading (objectID);
    }

    public DataContainer GetDataContainerWithLazyLoad (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();
      
      return _parentTransaction.DataManager.GetDataContainerWithLazyLoad (objectID);
    }

    public IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      CheckDisposed ();
      
      return _parentTransaction.DataManager.GetRelationEndPointWithoutLoading (relationEndPointID);
    }

    public bool IsInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();

      return _parentInvalidDomainObjectManager.IsInvalid (objectID);
    }

    public void MarkNotInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();

      _parentInvalidDomainObjectManager.MarkNotInvalid (objectID);
    }

    public void RegisterDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      CheckDisposed ();

      _parentTransaction.DataManager.RegisterDataContainer (dataContainer);
    }

    public IDataManagementCommand CreateDeleteCommand (DomainObject deletedObject)
    {
      ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
      CheckDisposed ();

      return _parentTransaction.DataManager.CreateDeleteCommand (deletedObject);
    }
      
    private void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException (GetType ().ToString ());
    }
  }
}