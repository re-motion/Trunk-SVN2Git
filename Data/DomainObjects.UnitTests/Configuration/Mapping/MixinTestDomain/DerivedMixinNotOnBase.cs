using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  public class DerivedMixinNotOnBase : MixinNotOnBase
  {
    public int DerivedMixinProperty
    {
      get { return Properties[typeof (DerivedMixinNotOnBase), "DerivedMixinProperty"].GetValue<int> (); }
    }
  }
}