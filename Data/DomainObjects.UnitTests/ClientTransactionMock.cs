using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [Serializable]
  public class ClientTransactionMock : RootClientTransaction
  {
    // types

    // static members and constants

    // member fields

    private int _numberOfCallsToLoadDataContainer;
    private int _numberOfCallsToLoadRelatedObject;

    // construction and disposing

    public ClientTransactionMock ()
    {
      Initialize ();
    }

    // methods and properties

    private void Initialize ()
    {
      _numberOfCallsToLoadDataContainer = 0;
      _numberOfCallsToLoadRelatedObject = 0;
    }

    protected override DomainObject LoadObject (ObjectID id)
    {
      _numberOfCallsToLoadDataContainer++;
      return base.LoadObject (id);
    }

    protected override DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      _numberOfCallsToLoadRelatedObject++;
      return base.LoadRelatedObject (relationEndPointID);
    }

    public new DomainObject GetObject (ObjectID id)
    {
      return base.GetObject (id);
    }

    public new DomainObject GetObject (ObjectID id, bool includeDeleted)
    {
      return base.GetObject (id, includeDeleted);
    }

    protected override ObjectList<T> GetObjects<T> (ObjectID[] objectIDs, bool throwOnNotFound)
    {
      return MockableGetObjects<T> (objectIDs, throwOnNotFound);
    }

    public virtual ObjectList<T> MockableGetObjects<T> (ObjectID[] objectIDs, bool throwOnNotFound) where T : DomainObject
    {
      return base.GetObjects<T> (objectIDs, throwOnNotFound);
    }

    public new DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
    {
      return base.GetRelatedObject (relationEndPointID);
    }

    public new DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return base.GetOriginalRelatedObjects (relationEndPointID);
    }

    public new DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return base.GetRelatedObjects (relationEndPointID);
    }

    public new void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
    {
      base.SetRelatedObject (relationEndPointID, newRelatedObject);
    }

    public new void SetClientTransaction (DataContainer dataContainer)
    {
      base.SetClientTransaction (dataContainer);
    }

    public int NumberOfCallsToLoadDataContainer
    {
      get { return _numberOfCallsToLoadDataContainer; }
    }

    public int NumberOfCallsToLoadRelatedObject
    {
      get { return _numberOfCallsToLoadRelatedObject; }
    }

    public new DataManager DataManager
    {
      get { return base.DataManager; }
    }

    public new bool IsReadOnly
    {
      get { return base.IsReadOnly; }
      set { base.IsReadOnly = value; }
    }

    public new void AddListener (IClientTransactionListener listener)
    {
      base.AddListener (listener);
    }
  }
}
