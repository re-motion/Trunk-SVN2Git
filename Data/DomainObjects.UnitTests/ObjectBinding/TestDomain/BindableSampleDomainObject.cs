using System;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [BindableDomainObject]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class BindableSampleDomainObject : DomainObject
  {
    public static BindableSampleDomainObject NewObject ()
    {
      return DomainObject.NewObject<BindableSampleDomainObject> ().With ();
    }

    public static BindableSampleDomainObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BindableSampleDomainObject> (id);
    }

    public abstract string Name { get; set; }
    public abstract int Int32 { get; set; }
  }
}
