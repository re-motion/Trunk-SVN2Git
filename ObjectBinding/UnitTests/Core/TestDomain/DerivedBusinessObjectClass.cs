using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [Serializable]
  public class DerivedBusinessObjectClass : BaseBusinessObjectClass
  {
    private string _public;

    public DerivedBusinessObjectClass ()
    {
    }

    public new string Public
    {
      get { return _public; }
      set { _public = value; }
    }

    private string Private
    {
      get { return null; }
      set { }
    }
  }
}