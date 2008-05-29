using System;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [DisableEnumValues (TestEnum.Value5)]
  public class ClassWithDisabledEnumValue
  {
    private TestEnum _disabledFromProperty;
    private TestEnum _disabledFromObject;

    public ClassWithDisabledEnumValue ()
    {
    }

    [DisableEnumValues(TestEnum.Value1)]
    public TestEnum DisabledFromProperty
    {
      get { return _disabledFromProperty; }
      set { _disabledFromProperty = value; }
    }

    public TestEnum DisabledFromObject
    {
      get { return _disabledFromObject; }
      set { _disabledFromObject = value; }
    }
  }
}