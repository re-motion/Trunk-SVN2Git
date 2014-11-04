using System;
using ActaNova.WebTesting.PageObjects;
using ActaNova.WebTesting.SampleTests;
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

      home = home.Tree.GetNode().WithIndex (2).Expand().GetNode().WithText ("egora Gemeinde").Select().Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("egora Gemeinde AV"));

      var geschaeftsfall = home.Tree.GetNode().WithText ("Favoriten").Expand().Collapse().Expand().GetNode().WithIndex (2);
      home = geschaeftsfall.Select().Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("Geschäftsfall \"OE/1/BW-BV-BA-M/1\" bearbeiten"));

      home =
          geschaeftsfall.GetNode ("Files")
              .Expand()
              .GetNode().WithIndex (1)
              .Expand()
              .GetNode ("WrappedDocumentsHierarchy")
              .Expand()
              .Select()
              .Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("Akt \"OE/1\" bearbeiten"));
    }
  }
}