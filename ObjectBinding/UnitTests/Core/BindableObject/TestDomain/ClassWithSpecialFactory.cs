using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  [UseSpecialMetadataFactory]
  [Serializable]
  public class ClassWithSpecialFactory
  {
    private string _string;
    private string _privateString;

    public ClassWithSpecialFactory ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }

    private string PrivateString
    {
      get { return _privateString; }
      set { _privateString = value; }
    }
  }
}