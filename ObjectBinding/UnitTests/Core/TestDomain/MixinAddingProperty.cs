using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public interface IMixinAddingProperty
  {
    string MixedProperty { get; set; }
    string MixedReadOnlyProperty { get; }
  }

  public class MixinAddingProperty : IMixinAddingProperty
  {
    private string _mixedProperty;

    public string MixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }

    public string MixedReadOnlyProperty
    {
      get { return _mixedProperty; }
    }
  }
}
