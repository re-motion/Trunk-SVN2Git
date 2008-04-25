using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
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