using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObjectWithIdentity]
  [Serializable]
  public class ClassWithIdentity
  {
    private string _string;
    private readonly string _uniqueIdentifier;

    public ClassWithIdentity (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    public ClassWithIdentity ()
      : this (Guid.NewGuid().ToString())
    {
    }

    [OverrideMixin]
    public string UniqueIdentifier
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