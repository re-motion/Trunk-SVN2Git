using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Serializable]
  public abstract class MixinWithSingleAbstractMethod : Mixin<object, object>
  {
    public int I;

    public string ImplementedMethod ()
    {
      return "MixinWithSingleAbstractMethod.ImplementedMethod-" + AbstractMethod (25);
    }

    protected abstract string AbstractMethod (int i);
  }

  [Serializable]
  public abstract class MixinWithSingleAbstractMethod2 : Mixin<object, object>
  {
    protected abstract string AbstractMethod (int i);
  }

}
