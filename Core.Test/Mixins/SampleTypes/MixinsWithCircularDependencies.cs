using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinWithCircularThisDependency1 : Mixin<ICircular2>, ICircular1
  {
    public string Circular1 ()
    {
      return "MixinWithCircularThisDependency1.Circular1-" + This.Circular2 ();
    }
  }

  public class MixinWithCircularThisDependency2 : Mixin<ICircular1>, ICircular2
  {
    public string Circular2 ()
    {
      return "MixinWithCircularThisDependency2.Circular2";
    }

    public string Circular12 ()
    {
      return "MixinWithCircularThisDependency2.Circular12-" + This.Circular1();
    }
  }

  public interface ICircular1
  {
    string Circular1 ();
  }

  public interface ICircular2
  {
    string Circular2 ();
    string Circular12 ();
  }
}