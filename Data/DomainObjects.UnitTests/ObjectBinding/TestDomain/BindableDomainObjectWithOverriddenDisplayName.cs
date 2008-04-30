using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  public abstract class BindableDomainObjectWithOverriddenDisplayName : BindableSampleDomainObject
  {
    public new static BindableDomainObjectWithOverriddenDisplayName NewObject ()
    {
      return NewObject<BindableDomainObjectWithOverriddenDisplayName> ().With ();
    }

    public static new BindableDomainObjectWithOverriddenDisplayName GetObject (ObjectID id)
    {
      return GetObject<BindableDomainObjectWithOverriddenDisplayName> (id);
    }

    [OverrideMixin]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}
