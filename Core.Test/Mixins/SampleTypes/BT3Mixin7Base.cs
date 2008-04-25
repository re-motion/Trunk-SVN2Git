using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Serializable]
  public class BT3Mixin7Base : Mixin<IBaseType31, ICBaseType3BT3Mixin4>
  {
    [OverrideTarget]
    public string IfcMethod()
    {
      return "BT3Mixin7Base.IfcMethod-" + Base.Foo() + "-" + ((IBaseType31)Base).IfcMethod() + "-" + Base.IfcMethod2();
    }
  }
}
