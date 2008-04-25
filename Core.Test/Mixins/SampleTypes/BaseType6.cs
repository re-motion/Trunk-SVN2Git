using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType6
  {
    string BaseMethod ();
  }

  public class BaseType6 : IBaseType6
  {
    public string BaseMethod ()
    {
      return "BaseType6.BaseMethod";
    }
  }
}
