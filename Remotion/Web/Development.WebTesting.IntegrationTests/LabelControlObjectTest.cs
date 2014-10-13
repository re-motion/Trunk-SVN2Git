using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class LabelControlObjectTes : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var label = home.GetLabel().ByID ("body_MySmartLabel");
      Assert.That (label.Scope.Id, Is.EqualTo ("body_MySmartLabel"));
    }

    [Test]
    [Ignore ("Ignored until labels have a CSS class - is this ever possible?")]
    // Todo RM-6297: enable test as soon as labels have a CSS class - is this ever possible?
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var label = home.GetLabel().ByIndex (2);
      Assert.That (label.Scope.Id, Is.EqualTo ("body_MySmartLabel2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var label = home.GetLabel().ByLocalID ("MySmartLabel");
      Assert.That (label.Scope.Id, Is.EqualTo ("body_MySmartLabel"));
    }

    [Test]
    [Ignore ("Ignored until labels have a CSS class - is this ever possible?")]
    // Todo RM-6297: enable test as soon as labels have a CSS class - is this ever possible?
    public void TestSelection_First ()
    {
      var home = Start();

      var label = home.GetLabel().First();
      Assert.That (label.Scope.Id, Is.EqualTo ("body_MySmartLabel"));
    }

    [Test]
    [Ignore ("Ignored until labels have a CSS class - is this ever possible?")]
    // Todo RM-6297: enable test as soon as labels have a CSS class - is this ever possible?
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var label = scope.GetLabel().Single();
      Assert.That (label.Scope.Id, Is.EqualTo ("body_MySmartLabel2"));

      try
      {
        home.GetLabel().Single();
        Assert.Fail ("Should not be able to unambigously find a label.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var label = home.GetLabel().ByLocalID ("MySmartLabel");
      Assert.That (label.GetText(), Is.EqualTo ("MySmartLabelContent"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("LabelTest.aspx");
    }
  }
}