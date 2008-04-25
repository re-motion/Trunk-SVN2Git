using System;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin1 : Mixin<IBaseType31, IBaseType31>
  {
    public new IBaseType31 This
    {
      get { return base.This; }
    }

    public new IBaseType31 Base
    {
      get { return base.Base; }
    }
  }

  [Serializable]
  public class BT3Mixin1B : Mixin<IBaseType31, IBaseType31>
  {
    public new IBaseType31 This
    {
      get { return base.This; }
    }

    public new IBaseType31 Base
    {
      get { return base.Base; }
    }
  }
}
