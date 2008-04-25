using System;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin2 : Mixin<IBaseType32>
  {
    public new IBaseType32 This
    {
      get { return base.This; }
    }
  }

  [Serializable]
  public class BT3Mixin2B : Mixin<IBaseType32>
  {
    public new IBaseType32 This
    {
      get { return base.This; }
    }
  }
}
