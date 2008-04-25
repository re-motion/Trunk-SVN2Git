using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  public class MixinNotOnBase : DomainObjectMixin<DomainObject>
  {
    public int MixinProperty
    {
      get { return Properties[typeof (MixinNotOnBase), "MixinProperty"].GetValue<int> (); }
    }
  }
}