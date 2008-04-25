using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IDuckBaseRequirements
  {
    string MethodImplementedOnBase ();
    string ProtectedMethodImplementedOnBase ();
  }

  public class DuckBaseMixin : Mixin<object, IDuckBaseRequirements>
  {
    [OverrideTarget]
    public string MethodImplementedOnBase ()
    {
      return "DuckBaseMixin.MethodImplementedOnBase-" + Base.MethodImplementedOnBase ();
    }

    [OverrideTarget]
    public string ProtectedMethodImplementedOnBase ()
    {
      return "DuckBaseMixin.ProtectedMethodImplementedOnBase-" + Base.ProtectedMethodImplementedOnBase ();
    }
  }

  public class DuckBaseMixinWithoutOverrides : Mixin<object, IDuckBaseRequirements>
  {
  }
}
