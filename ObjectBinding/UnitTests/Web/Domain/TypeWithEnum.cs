using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithEnum
  {
    public static TypeWithEnum Create ()
    {
      return ObjectFactory.Create<TypeWithEnum> (true).With ();
    }

    private TestEnum _enumValue;

    protected TypeWithEnum ()
    {
    }

    public TestEnum EnumValue
    {
      get { return _enumValue; }
      set { _enumValue = value; }
    }
  }

  public enum TestEnum
  {
    First,
    Second,
    Third
  }
}