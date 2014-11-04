using System;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaBreadCrumbControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (1));
      Assert.That (home.Header.GetBreadCrumbs()[0].GetText(), Is.EqualTo ("Eigener AV"));

      home.Tree.GetNode().WithText ("Stellvertretungs AV").Select();

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (1));
      Assert.That (home.Header.GetBreadCrumbs()[0].GetText(), Is.EqualTo ("Stellvertretungs AV"));

      home.MainMenu.Select ("Verfahrensbereich", "BA - Bürgeranliegen");
      home.MainMenu.Select ("Extras", "Passwort ändern");

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (2));
      Assert.That (home.Header.GetBreadCrumbs()[0].GetText(), Is.EqualTo ("Stellvertretungs AV"));
      Assert.That (home.Header.GetBreadCrumbs()[1].GetText(), Is.EqualTo ("Passwort ändern"));

      var searchPage = home.MainMenu.Select ("Suchen", "Geschäftsobjekt", "Akt").ExpectActaNova();

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (3));
      Assert.That (home.Header.GetBreadCrumbs()[2].GetText(), Is.EqualTo ("Akt Suchen"));

      searchPage.FormPage.Perform ("Search");

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (3));
      Assert.That (home.Header.GetBreadCrumbs()[2].GetText(), Is.EqualTo ("Akt Suchen"));

      searchPage.Header.GetBreadCrumbs()[0].Click();

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (1));
      Assert.That (home.Header.GetBreadCrumbs()[0].GetText(), Is.EqualTo ("Stellvertretungs AV"));
    }
  }
}