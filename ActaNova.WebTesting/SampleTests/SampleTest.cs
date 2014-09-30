using System;
using ActaNova.WebTesting.PageObjects;
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

      var newCitizenConcernPage = home.MainMenu.Select ("New", "Citizen concern").Expect<ActaNovaMainPageObject>();
    }
  }
}