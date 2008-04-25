using System;
using NUnit.Framework;
using Assertion=Remotion.Utilities.Assertion;
using AssertionException=Remotion.Utilities.AssertionException;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AssertionTest
  {
    [Test]
    public void TestIsTrueHolds ()
    {
      Assertion.IsTrue (true);
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void TestIsTrueFails ()
    {
      Assertion.IsTrue (false);
    }
  }
}
