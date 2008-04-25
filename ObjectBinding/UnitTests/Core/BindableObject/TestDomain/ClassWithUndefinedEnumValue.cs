using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithUndefinedEnumValue
  {
    private EnumWithUndefinedValue _scalar;
    private EnumWithUndefinedValue[] _array;

    public ClassWithUndefinedEnumValue ()
    {
    }

    public EnumWithUndefinedValue Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
    }

    public EnumWithUndefinedValue[] Array
    {
      get { return _array; }
      set { _array = value; }
    }
  }
}