using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class StubBusinessObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public StubBusinessObjectProviderAttribute ()
        : base (typeof (StubBusinessObjectProvider))
    {
    }
  }
}