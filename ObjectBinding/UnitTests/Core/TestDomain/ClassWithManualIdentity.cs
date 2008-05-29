using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [Serializable]
  public class ClassWithManualIdentity : ManualBusinessObject, IBusinessObjectWithIdentity
  {
    private readonly string _uniqueIdentifier;

    public ClassWithManualIdentity (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }
  }
}