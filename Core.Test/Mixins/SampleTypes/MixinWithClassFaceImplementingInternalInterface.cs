using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  internal interface IInternalInterface1
  {
    string MethodInInternalInterface ();
  }

  internal interface IInternalInterface2
  {
  }

  [Uses (typeof (MixinWithClassFaceImplementingInternalInterface))]
  public class ClassImplementingInternalInterface : IInternalInterface1, IInternalInterface2
  {
    public string MethodInInternalInterface ()
    {
      return "ClassImplementingInternalInterface.Foo";
    }
  }

  public class MixinWithClassFaceImplementingInternalInterface : Mixin <ClassImplementingInternalInterface>
  {
    public string GetStringViaThis ()
    {
      return This.MethodInInternalInterface ();
    }
  }
}
