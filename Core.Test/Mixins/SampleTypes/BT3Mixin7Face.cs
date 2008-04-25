using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Serializable]
  public class BT3Mixin7Face : Mixin<ICBaseType3BT3Mixin4>
  {
    public string InvokeThisMethods()
    {
      return ((IBaseType31)This).IfcMethod() + "-" + This.Foo();
    }
  }
}
