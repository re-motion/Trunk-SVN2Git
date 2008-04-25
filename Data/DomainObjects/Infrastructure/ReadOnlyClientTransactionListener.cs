using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// An implementation of <see cref="IClientTransactionListener"/> which throws an exception if the <see cref="ClientTransaction"/> is about
  /// to be modified while in a read-only state.
  /// </summary>
  [Serializable]
  public class ReadOnlyClientTransactionListener : IClientTransactionListener
  {
    private readonly ClientTransaction _clientTransaction;

    public ReadOnlyClientTransactionListener (ClientTransaction clientTransaction)
    {
      _clientTransaction = clientTransaction;
    }

    private void EnsureWriteable (string operation)
    {
      if (_clientTransaction.IsReadOnly)
      {
        string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is read-only. "
            + "Offending transaction modification: {0}.",
            operation);
        throw new ClientTransactionReadOnlyException (message);
      }
    }

    public virtual void SubTransactionCreating ()
    {
      EnsureWriteable ("SubTransactionCreating");
    }

    public virtual void SubTransactionCreated (ClientTransaction subTransaction)
    {
      Assertion.IsTrue (_clientTransaction.IsReadOnly); // after a subtransaction has been created, the parent must be read-only
    }

    public virtual void NewObjectCreating (Type type, DomainObject instance)
    {
      EnsureWriteable ("NewObjectCreating");
    }

    public virtual void ObjectLoading (ObjectID id)
    {
      EnsureWriteable ("ObjectLoading");
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      EnsureWriteable ("ObjectInitializedFromDataContainer");
    }

    public virtual void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void ObjectDeleting (DomainObject domainObject)
    {
      EnsureWriteable ("ObjectDeleting");
    }

    public virtual void ObjectDeleted (DomainObject domainObject)
    {
      Assertion.IsFalse(_clientTransaction.IsReadOnly);
    }

    public virtual void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      EnsureWriteable ("PropertyValueChanging");
    }

    public virtual void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      EnsureWriteable ("RelationChanging");
    }

    public virtual void RelationChanged (DomainObject domainObject, string propertyName)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
    }

    public virtual void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      EnsureWriteable ("TransactionCommitting");
    }

    public virtual void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      EnsureWriteable ("TransactionRollingBack");
    }

    public virtual void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      EnsureWriteable ("RelationEndPointMapRegistering");
    }

    public virtual void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
    }

    public virtual void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void DataManagerCopyingFrom (DataManager source)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void DataManagerCopyingTo (DataManager destination)
    {
    }

    public virtual void DataContainerMapRegistering (DataContainer container)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void DataContainerMapUnregistering (DataContainer container)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public virtual void DataContainerMapCopyingTo (DataContainerMap destination)
    {
    }
  }
}