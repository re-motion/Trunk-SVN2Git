using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinIII1
  {
    string Method1();
  }

  public interface IMixinIII2 : IMixinIII1
  {
    string Method2 ();
  }

  public interface IMixinIII3 : IMixinIII1, IMixinIII2
  {
    string Method3 ();
  }

  public interface IMixinIII4 : IMixinIII3
  {
    string Method4 ();
  }

  public class MixinIntroducingInheritedInterface : IMixinIII4
  {
    public string Method4 ()
    {
      return "MixinIntroducingInheritedInterface.Method4";
    }

    public string Method3 ()
    {
      return "MixinIntroducingInheritedInterface.Method3";
    }

    public string Method1 ()
    {
      return "MixinIntroducingInheritedInterface.Method1";
    }

    public string Method2 ()
    {
      return "MixinIntroducingInheritedInterface.Method2";
    }
  }
}
