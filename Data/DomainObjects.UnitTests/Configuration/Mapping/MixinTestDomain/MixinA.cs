using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  public class MixinA : MixinBase
  {
    public int P5
    {
      get { return Properties[typeof (MixinA), "P5"].GetValue<int>(); }
    }
  }
}