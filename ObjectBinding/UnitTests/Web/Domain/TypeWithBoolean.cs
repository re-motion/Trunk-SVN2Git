using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithBoolean
  {
    public static TypeWithBoolean Create ()
    {
      return ObjectFactory.Create<TypeWithBoolean> (true).With ();
    }

    private bool _booleanValue;
    private bool? _nullableBooleanValue;

    protected TypeWithBoolean ()
    {
    }

    public bool BooleanValue
    {
      get { return _booleanValue; }
      set { _booleanValue = value; }
    }

    public bool? NullableBooleanValue
    {
      get { return _nullableBooleanValue; }
      set { _nullableBooleanValue = value; }
    }
  }
}