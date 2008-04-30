using System;
using Remotion.Data.DomainObjects.ObjectBinding;

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
      return NewObject<BindableSampleDomainObject> ().With ();
    }

    public static BindableSampleDomainObject GetObject (ObjectID id)
    {
      return GetObject<BindableSampleDomainObject> (id);
    }

    public abstract string Name { get; set; }
    public abstract int Int32 { get; set; }
  }
}
