using System;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class CommonCollectionTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CommonCollectionTest ()
    {
    }

    // methods and properties

    [Test]
    public void ContainsNullForKeyNotInCollection ()
    {
      CommonCollectionMock collection = new CommonCollectionMock ();

      Assert.IsFalse (collection.Contains ("invalidKey", null));
    }
  }
}
