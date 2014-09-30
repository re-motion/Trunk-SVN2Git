using System;
using NUnit.Framework;

namespace ActaNova.WebTesting.SampleTests
{
  [TestFixture]
  public class SampleTest : ActaNovaWebTestBase
  {
    [Test]
    public void MySampleTest ()
    {
      var home = Start();
    }
  }
}