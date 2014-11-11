using ActaNova.WebTesting.ActaNovaExtensions;
using NUnit.Framework;

namespace ActaNova.WebTesting.ActaNovaExtensionsTest
{
  [TestFixture]
  public class ActaNovaClickExtensionsTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestBreadCrumbs ()
    {
      var home = Start();

      home = home.MainMenu.Neu_Eingangsstueck();

      home.Header.GetBreadCrumbs()[0].ClickAndPreventDataLoss();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingangsstück erzeugen"));

      home = home.Header.GetBreadCrumbs()[0].ClickAndConfirmDataLoss().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestActaNovaTreeNode_CurrentNode ()
    {
      var home = Start();

      home = home.MainMenu.Neu_Eingangsstueck();

      home.Tree.GetEigenerAvNode().SelectAndPreventDataLoss();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingangsstück erzeugen"));

      home = home.Tree.GetEigenerAvNode().SelectAndConfirmDataLoss().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestActaNovaTreeNode_OtherNode ()
    {
      var home = Start();

      home = home.MainMenu.Neu_Eingangsstueck();

      home.Tree.GetGruppenAvNode().SelectAndPreventDataLoss();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingangsstück erzeugen"));

      home = home.Tree.GetGruppenAvNode().SelectAndConfirmDataLoss().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen AV"));
    }

    [Test]
    public void TestAppToolsFormPage ()
    {
      var home = Start();

      home = home.MainMenu.Neu_Eingangsstueck();

      home.FormPage.PerformAndPreventDataLoss ("Cancel").ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingangsstück erzeugen"));

      home = home.FormPage.PerformAndConfirmDataLoss ("Cancel").ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }
  }
}