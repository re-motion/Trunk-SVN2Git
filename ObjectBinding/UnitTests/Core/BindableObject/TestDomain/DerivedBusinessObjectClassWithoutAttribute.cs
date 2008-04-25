using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [Serializable]
  public class DerivedBusinessObjectClassWithoutAttribute : BaseBusinessObjectClass
  {
    private string _public;

    public DerivedBusinessObjectClassWithoutAttribute ()
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