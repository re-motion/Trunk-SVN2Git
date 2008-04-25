using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class InheritableAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class NonInheritableAttribute : Attribute
  {
  }

  [Inheritable]
  [NonInheritable]
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class MixinWithProtectedOverriderAndAttributes
  {
    [OverrideTarget]
    protected new string ToString ()
    {
      return "";
    }

    [SampleCopyTemplate]
    public class CopyTemplate {}
  }
}