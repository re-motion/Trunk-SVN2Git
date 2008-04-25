using System;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObjectWithIdentity]
  [GetObjectServiceType (typeof (ICustomGetObjectService))]
  public class ClassWithIdentityAndGetObjectServiceAttribute
  {
    private readonly string _uniqueIdentifier;

    public ClassWithIdentityAndGetObjectServiceAttribute (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    [OverrideMixin]
    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }
  }
}