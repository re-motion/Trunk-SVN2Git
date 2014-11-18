using System;
using ActaNova.WebTesting.PageObjects;
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

      home = home.MainMenu.Select ("Extras", "Benutzerprofil").ExpectMainPage();
      home = home.Refresh().ExpectMainPage();

      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestGetFrame ()
    {
      var home = Start();

      Assert.That (home.GetFrame<ActaNovaWorkListPageObject>().GetTitle(), Is.EqualTo ("Eigener AV"));
    }
  }
}