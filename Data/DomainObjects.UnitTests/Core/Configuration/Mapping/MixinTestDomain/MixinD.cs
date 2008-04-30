using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain
{
  public class MixinD : DomainObjectMixin<DomainObject>
  {
    public int P8
    {
      get { return Properties[typeof (MixinD), "P8"].GetValue<int> (); }
    }
  }
}