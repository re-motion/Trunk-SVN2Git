using System;
using System.Collections;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class EqualityUtilityTest
  {
    [Test]
    public void GetRotatedHashCode_ForEnumerable()
    {
      IEnumerable objects1 = new int[] {1, 2, 3};
      IEnumerable objects2 = new int[] {1, 2, 3};
      Assert.AreEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects2));

      IEnumerable objects3 = new int[] {3, 2, 1};
      Assert.AreNotEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects3));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.AreNotEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects4));
    }

    [Test]
    public void GetXorHashCode ()
    {
      IEnumerable objects1 = new int[] { 1, 2, 3 };
      IEnumerable objects2 = new int[] { 1, 2, 3 };
      Assert.AreEqual (EqualityUtility.GetXorHashCode (objects1), EqualityUtility.GetXorHashCode (objects2));

      IEnumerable objects3 = new int[] { 3, 2, 1 };
      Assert.AreEqual (EqualityUtility.GetXorHashCode (objects1), EqualityUtility.GetXorHashCode (objects3));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.AreNotEqual (EqualityUtility.GetXorHashCode (objects1), EqualityUtility.GetXorHashCode (objects4));
    }
  }
}
