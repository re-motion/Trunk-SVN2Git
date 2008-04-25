using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinWithEmptyInterface { }
  public class MixinRequiringEmptyInterface : Mixin<object, IMixinWithEmptyInterface> { }
}