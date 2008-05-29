using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObjectWithIdentity]
  [Serializable]
  public class ClassWithIdentityAndDisplayName
  {
    private string _string;
    private readonly string _uniqueIdentifier;

    public ClassWithIdentityAndDisplayName (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    public ClassWithIdentityAndDisplayName ()
      : this (Guid.NewGuid().ToString())
    {
    }

    [OverrideMixin]
    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    [OverrideMixin]
    public string DisplayName
    {
      get { return _uniqueIdentifier; }
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}