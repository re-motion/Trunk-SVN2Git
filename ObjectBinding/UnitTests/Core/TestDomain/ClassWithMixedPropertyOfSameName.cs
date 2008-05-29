using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [Uses (typeof (MixinAddingProperty))]
  public class ClassWithMixedPropertyOfSameName
  {
    private string _mixedProperty;

    public string MixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }
  }
}
