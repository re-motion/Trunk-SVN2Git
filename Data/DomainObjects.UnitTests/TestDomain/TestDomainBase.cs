using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

#pragma warning disable 0618

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public abstract class TestDomainBase : DomainObject
  {
    public static TestDomainBase GetObject (ObjectID id)
    {
      return DomainObject.GetObject<TestDomainBase> (id);
    }

    public static TestDomainBase GetObject (ObjectID id, bool includeDeleted)
    {
      return DomainObject.GetObject<TestDomainBase> (id, includeDeleted);
    }

    protected TestDomainBase()
    {
    }

    protected TestDomainBase (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    [StorageClassNone]
    public DataContainer InternalDataContainer
    {
      get { return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, typeof (DomainObject), "GetDataContainerForTransaction", ClientTransaction); }
    }

    public DataContainer GetInternalDataContainerForTransaction(ClientTransaction transaction)
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, "GetDataContainerForTransaction", transaction);
    }

    public DomainObject GetRelatedObject (string propertyName)
    {
      return (DomainObject) Properties[propertyName].GetValueWithoutTypeCheck ();
    }

    public DomainObjectCollection GetRelatedObjects (string propertyName)
    {
      return (DomainObjectCollection) Properties[propertyName].GetValueWithoutTypeCheck ();
    }

    public DomainObject GetOriginalRelatedObject (string propertyName)
    {
      return (DomainObject) Properties[propertyName].GetOriginalValueWithoutTypeCheck ();
    }

    public DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
    {
      return (DomainObjectCollection) Properties[propertyName].GetOriginalValueWithoutTypeCheck ();
    }

    public void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
    {
      Properties[propertyName].SetValueWithoutTypeCheck (newRelatedObject);
    }

    public new void Delete ()
    {
      base.Delete();
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    public new DomainObjectGraphTraverser GetGraphTraverser(IGraphTraversalStrategy stragety)
    {
      return base.GetGraphTraverser (stragety);
    }
  }
}
#pragma warning restore 0618