using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.MixerTool.SampleAssembly
{
  public class BaseType
  {
  }

  [Extends (typeof (BaseType))]
  public class Mixin
  {
  }
}