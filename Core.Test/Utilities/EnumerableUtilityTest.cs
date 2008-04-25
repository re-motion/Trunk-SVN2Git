using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class EnumerableUtilityTest
  {
    [Test]
    public void Cast()
    {
      int[] sourceArray = new int[] {1, 2, 3};
      IEnumerable sourceEnumerable = sourceArray;
      IEnumerable<int> castEnumerable1 = EnumerableUtility.Cast<int> (sourceEnumerable);
      IEnumerable<object> castEnumerable2 = EnumerableUtility.Cast<object> (sourceEnumerable);
      Assert.IsNotNull (castEnumerable1);
      Assert.IsNotNull (castEnumerable2);
      Assert.That (EnumerableUtility.ToArray (castEnumerable1), Is.EqualTo (sourceArray));
      Assert.That (EnumerableUtility.ToArray (castEnumerable2), Is.EqualTo (sourceArray));
    }

    [Test]
    public void CombineToArray ()
    {
      int[] combined = EnumerableUtility.CombineToArray (new int[] {1, 2, 3}, new List<int> (new int[] {3, 4, 5}));
      Assert.That (combined, Is.EqualTo (new int[] {1, 2, 3, 3, 4, 5}));
    }
  }
}