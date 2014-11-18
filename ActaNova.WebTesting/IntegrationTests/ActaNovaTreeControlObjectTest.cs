using System;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaTreeControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var eigenerAv = home.Tree.GetNode().WithIndex (1);
      Assert.That (eigenerAv.GetText(), Is.EqualTo ("Eigener AV"));

      home = home.Tree.GetNode().WithIndex (2).Expand().GetNode().WithDisplayText ("egora Gemeinde").Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("egora Gemeinde AV"));

      var geschaeftsfall = home.Tree.GetNode().WithDisplayText ("Favoriten").Expand().Collapse().Expand().GetNode().WithIndex (2);
      home = geschaeftsfall.Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Geschäftsfall \"OE/1/BW-BV-BA-M/1\" bearbeiten"));

      home =
          geschaeftsfall.GetNode ("Files")
              .Expand()
              .GetNode().WithIndex (1)
              .Expand()
              .GetNode ("WrappedDocumentsHierarchy")
              .Expand()
              .Select()
              .ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Akt \"OE/1\" bearbeiten"));
    }

    [Test]
    public void TestGetMethods ()
    {
      var home = Start();

      var eigenerAvNode = home.Tree.GetNode().WithDisplayText ("Eigener AV");
      Assert.That (eigenerAvNode.IsSelected(), Is.True);

      var gruppenAvNode = home.Tree.GetNode().WithDisplayText ("Gruppen AV");
      Assert.That (gruppenAvNode.IsSelected(), Is.False);

      gruppenAvNode.Select();
      Assert.That (eigenerAvNode.IsSelected(), Is.False);
      Assert.That (gruppenAvNode.IsSelected(), Is.True);

      Assert.That (eigenerAvNode.GetNumberOfChildren(), Is.EqualTo (0));
      Assert.That (gruppenAvNode.GetNumberOfChildren(), Is.EqualTo (3));
    }

    [Test]
    public void TestDoubleSelection ()
    {
      var home = Start();

      home = home.Tree.GetNode().WithIndex (1).Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.Tree.GetNode().WithIndex (1).Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }
  }
}