using System;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class Stub2BusinessObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public Stub2BusinessObjectProviderAttribute ()
        : base (typeof (StubBusinessObjectProvider))
    {
    }
  }
}