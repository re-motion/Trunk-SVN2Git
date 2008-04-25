using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class ClassWithCopyCustomAttributes
  {
    [SampleCopyTemplate]
    public class CopyTemplate { }
  }
}
