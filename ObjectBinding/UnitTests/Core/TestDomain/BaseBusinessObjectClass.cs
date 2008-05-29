using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [Serializable]
  public class BaseBusinessObjectClass
  {
    private object _public;

    public BaseBusinessObjectClass ()
    {
    }

    public object Public
    {
      get { return _public; }
      set { _public = value; }
    }
  }
}