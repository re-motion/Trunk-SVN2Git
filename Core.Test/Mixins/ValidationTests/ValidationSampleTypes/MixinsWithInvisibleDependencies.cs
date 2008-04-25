using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  interface IInvisible { }

  class MixinWithInvisibleThisDependency : Mixin<IInvisible>
  {
  }

  class MixinWithInvisibleBaseDependency : Mixin<object, IInvisible>
  {
  }
}
