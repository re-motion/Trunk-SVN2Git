using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class OtherBusinessObjectImplementationProviderAttribute : BusinessObjectProviderAttribute
  {
    public OtherBusinessObjectImplementationProviderAttribute ()
        : base (typeof (OtherBusinessObjectImplementationProvider))
    {
    }
  }
}