using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain
{
  public class MixinE : DomainObjectMixin<DomainObject>
  {
    public int P9
    {
      get { return Properties[typeof (MixinE), "P9"].GetValue<int> (); }
    }
  }
}