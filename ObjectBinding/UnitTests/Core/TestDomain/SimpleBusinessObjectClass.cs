using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [Serializable]
  public class SimpleBusinessObjectClass
  {
    private string _string;

    public SimpleBusinessObjectClass ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }

    public string StringWithoutGetter
    {
      set { _string = value; }
    }

    public string StringWithoutSetter
    {
      set { _string = value; }
    }
  }
}