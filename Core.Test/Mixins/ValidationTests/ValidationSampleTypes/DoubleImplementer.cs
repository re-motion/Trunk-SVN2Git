using System;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class DoubleImplementer : IBaseType2
  {
    public string IfcMethod ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}