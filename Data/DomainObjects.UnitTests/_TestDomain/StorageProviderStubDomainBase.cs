using System;
using Remotion.Data.DomainObjects.Infrastructure;

#pragma warning disable 0618

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public abstract class StorageProviderStubDomainBase : DomainObject
  {
    protected StorageProviderStubDomainBase ()
    {
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

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }
  }
}
#pragma warning restore 0618