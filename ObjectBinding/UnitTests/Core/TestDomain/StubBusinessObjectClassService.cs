using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class StubBusinessObjectClassService : IBusinessObjectClassService
  {
    public StubBusinessObjectClassService ()
    {
    }

    public IBusinessObjectClass GetBusinessObjectClass (Type type)
    {
      return null;
    }
  }
}