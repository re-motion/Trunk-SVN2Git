using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public interface IMixinAddingProperty
  {
    string MixedProperty { get; set; }
  }

  public class MixinAddingProperty : IMixinAddingProperty
  {
    private string _mixedProperty;

    public string MixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }
  }
}
