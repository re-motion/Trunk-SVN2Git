using System;
using System.Collections.Generic;
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

    private bool _disposed;

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

    public DomainObject[] GetObjects (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
      CheckDisposed ();
      
      return _parentTransaction.GetObjects<DomainObject> (objectIDs);
    }

    public DomainObject TryGetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed();

      return _parentTransaction.TryGetObject (objectID);
    }

    public DomainObject[] TryGetObjects (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
      CheckDisposed ();

      return _parentTransaction.TryGetObjects<DomainObject> (objectIDs);
    }

    public DomainObject ResolveRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      CheckDisposed ();
      if (!relationEndPointID.Definition.IsVirtual || relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException ("EndPoint ID must denote a virtual relation end-point with cardinality one.", "relationEndPointID");

      var endPoint = (IVirtualObjectEndPoint) _parentTransaction.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      return endPoint.GetData();
    }

    public IEnumerable<DomainObject> GetRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      CheckDisposed ();
      if (!relationEndPointID.Definition.IsVirtual || relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("EndPoint ID must denote a virtual relation end-point with cardinality many.", "relationEndPointID");

      var endPoint = (ICollectionEndPoint) _parentTransaction.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      return endPoint.GetData();
    }

    public QueryResult<DomainObject> ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      CheckDisposed ();
      
      return _parentTransaction.QueryManager.GetCollection (query);
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      CheckDisposed ();

      return _parentTransaction.QueryManager.GetCustom (query, qrr => qrr);
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

    public DataContainer GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();
      
      return _parentTransaction.DataManager.GetDataContainerWithLazyLoad (objectID, throwOnNotFound);
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

    public void Discard (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      CheckDisposed();

      _parentTransaction.DataManager.Discard (dataContainer);
    }

    private void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException (GetType ().ToString ());
    }
  }
}