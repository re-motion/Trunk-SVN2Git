using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class StubBusinessObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public StubBusinessObjectProviderAttribute ()
        : base (typeof (StubBusinessObjectProvider))
    {
    }
  }
}