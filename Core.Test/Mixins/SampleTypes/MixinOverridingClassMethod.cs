using System;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Serializable]
  public class MixinOverridingClassMethod : Mixin<object, MixinOverridingClassMethod.IRequirements>, IMixinOverridingClassMethod
  {
    public interface IRequirements
    {
      string OverridableMethod (int i);
    }

    public new object This { get { return base.This; } }
    public new object Base { get { return base.Base; } }

    [OverrideTarget]
    public string OverridableMethod (int i)
    {
      return "MixinOverridingClassMethod.OverridableMethod-" + i;
    }

    public virtual string AbstractMethod (int i)
    {
      return "MixinOverridingClassMethod.AbstractMethod-" + i;
    }
  }

  public interface IMixinOverridingClassMethod
  {
    string AbstractMethod (int i);
  }
}
