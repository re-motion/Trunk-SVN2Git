using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IIndirectRequirementBase1
  {
    string BaseMethod1 ();
  }

  public interface IIndirectRequirement1 : IIndirectRequirementBase1
  {
    string Method1 ();
  }

  public interface IIndirectRequirementBase2
  {
  }

  public interface IIndirectRequirement2 : IIndirectRequirementBase2
  {
  }

  public interface IIndirectRequirementBase3
  {
    string Method3 ();
  }

  public interface IIndirectRequirement3 : IIndirectRequirementBase3
  {
  }

  // This dependencies can contain unfulfilled dependencies, e.g. IIndirectRequirement2
  public interface IIndirectThisAggregator : IIndirectRequirement1, IIndirectRequirement2, IIndirectRequirement3
  {
  }

  // Base dependencies cannot contain unfulfilled dependencies, e.g. IIndirectRequirement2
  public interface IIndirectBaseAggregator : IIndirectRequirement1, IIndirectRequirement3
  {
  }

  public class MixinWithIndirectRequirements : Mixin <IIndirectThisAggregator, IIndirectBaseAggregator>
  {
    public string GetStuffViaThis()
    {
      return This.Method1() + "-" + This.BaseMethod1() + "-" + This.Method3();
    }

    public string GetStuffViaBase()
    {
      return Base.Method1() + "-" + Base.BaseMethod1() + "-" + Base.Method3();
    }
  }

  [Uses (typeof (MixinWithIndirectRequirements))]
  public class ClassImplementingIndirectRequirements : IIndirectRequirement1, IIndirectRequirement3
  {
    public string Method1 ()
    {
      return "ClassImplementingIndirectRequirements.Method1";
    }

    public string BaseMethod1 ()
    {
      return "ClassImplementingIndirectRequirements.BaseMethod1";
    }

    public string Method3 ()
    {
      return "ClassImplementingIndirectRequirements.Method3";

    }
  }
}
