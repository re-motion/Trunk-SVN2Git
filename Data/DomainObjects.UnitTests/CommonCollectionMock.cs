using System;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class CommonCollectionMock : CommonCollection
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CommonCollectionMock ()
    {
    }

    // methods and properties

    public bool Contains (object key, object value)
    {
      return BaseContains (key, value);
    }

  }
}
