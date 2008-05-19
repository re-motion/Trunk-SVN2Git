using System;
using System.Diagnostics;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  [CopyCustomAttributes(typeof (AttributeSource))]
  public class MixinAddingDebuggerDisplay
  {
    [DebuggerDisplay("Y")]
    private class AttributeSource { }
  }
}