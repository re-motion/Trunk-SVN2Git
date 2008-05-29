using System;
using Remotion.Globalization;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [MultiLingualResources ("Remotion.ObjectBinding.UnitTests.Core.Globalization.ClassWithResources")]
  public class ClassWithResources
  {
    private string _value1;
    private string _valueWithoutResource;

    public ClassWithResources ()
    {
    }
    
    public string Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }

    public string ValueWithoutResource
    {
      get { return _valueWithoutResource; }
      set { _valueWithoutResource = value; }
    }
  }
}