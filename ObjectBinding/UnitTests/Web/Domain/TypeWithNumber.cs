using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithNumber
  {
    public static TypeWithNumber Create ()
    {
      return ObjectFactory.Create<TypeWithNumber> (true).With ();
    }

    private int _int32Value;
    private int? _nullableInt32Value;

    protected TypeWithNumber ()
    {
    }

    public int Int32Value
    {
      get { return _int32Value; }
      set { _int32Value = value; }
    }

    public int? NullableInt32Value
    {
      get { return _nullableInt32Value; }
      set { _nullableInt32Value = value; }
    }
  }
}