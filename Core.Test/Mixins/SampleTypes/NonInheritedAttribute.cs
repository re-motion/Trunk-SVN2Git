using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class NonInheritedAttribute : Attribute
  {
    public NonInheritedAttribute ()
    {
    }
  }
}