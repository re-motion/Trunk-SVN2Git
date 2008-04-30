using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain
{
  public class MixinBase : DomainObjectMixin<DomainObject>
  {
    public int P0a
    {
      get { return Properties[typeof (MixinBase), "P0a"].GetValue<int> (); }
    }
  }
}