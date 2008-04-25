using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  public class MixinC : DomainObjectMixin<DomainObject>
  {
    public int P7
    {
      get { return Properties[typeof (MixinC), "P7"].GetValue<int>(); }
    }
  }
}