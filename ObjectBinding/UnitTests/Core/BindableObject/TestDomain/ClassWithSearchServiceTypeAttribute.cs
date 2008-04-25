using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  [SearchAvailableObjectsServiceType (typeof (ISearchServiceOnType))]
  public class ClassWithSearchServiceTypeAttribute
  {
    public ClassWithSearchServiceTypeAttribute ()
    {
    }
  }
}