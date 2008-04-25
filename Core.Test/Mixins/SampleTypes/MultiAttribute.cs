using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class MultiAttribute : Attribute { }
}