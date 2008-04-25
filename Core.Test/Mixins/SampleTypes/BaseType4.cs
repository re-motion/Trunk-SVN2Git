using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType4 {}

  // no attributes
  public class BaseType4 : IBaseType4
  {
    public string NonVirtualMethod ()
    {
      return "BaseType4.NonVirtualMethod";
    }

    public string NonVirtualProperty
    {
      get { return "BaseType4.NonVirtualProperty"; }
    }

    public event EventHandler NonVirtualEvent;
  }
}
