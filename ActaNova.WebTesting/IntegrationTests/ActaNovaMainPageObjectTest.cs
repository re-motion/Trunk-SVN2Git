using System;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaMainPageObjectTest : ActaNovaWebTestBase
  {
    // Note: contains additional tests which are not covered by the control object integration tests.

    [Test]
    public void TestRefreshButton ()
    {
      var home = Start();

      home = home.MainMenu.Select ("Extras", "Benutzerprofil").ExpectActaNova();
      home = home.Refresh().ExpectActaNova();

      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }
  }
}