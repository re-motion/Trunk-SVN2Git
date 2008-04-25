using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithOverriddenDisplayName
  {
    public ClassWithOverriddenDisplayName ()
    {
    }

    [OverrideMixin]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}