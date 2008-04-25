using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class GenericTargetClass<T>
  {
    public virtual T VirtualMethod ()
    {
      return default (T);
    }
  }
}