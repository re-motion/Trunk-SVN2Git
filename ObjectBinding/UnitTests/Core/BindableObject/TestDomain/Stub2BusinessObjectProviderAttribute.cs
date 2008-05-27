using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class Stub2BusinessObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public Stub2BusinessObjectProviderAttribute ()
        : base (typeof (StubBusinessObjectProvider))
    {
    }
  }
}