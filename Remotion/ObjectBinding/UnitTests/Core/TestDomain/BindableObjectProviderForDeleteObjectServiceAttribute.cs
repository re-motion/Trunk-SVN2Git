using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class BindableObjectProviderForDeleteObjectServiceAttribute : BusinessObjectProviderAttribute
  {
    public BindableObjectProviderForDeleteObjectServiceAttribute ()
        : base (typeof (BindableObjectProvider))
    {
    }
  }
}