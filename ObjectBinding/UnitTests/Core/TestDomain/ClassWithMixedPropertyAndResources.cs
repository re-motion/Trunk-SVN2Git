using System;
using Remotion.Globalization;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [MultiLingualResources ("Remotion.ObjectBinding.UnitTests.Core.Globalization.ClassWithMixedPropertyAndResources")]
  [Uses (typeof (MixinAddingProperty))]
  public class ClassWithMixedPropertyAndResources
  {
    private string _value1;

    public string Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }
  }
}
