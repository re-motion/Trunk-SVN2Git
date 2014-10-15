using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class LabelControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var smartLabel = home.GetLabel().ByID ("body_MySmartLabel");
      Assert.That (smartLabel.Scope.Id, Is.EqualTo ("body_MySmartLabel"));

      var formGridLabel = home.GetLabel().ByID ("body_MyFormGridLabel");
      Assert.That (formGridLabel.Scope.Id, Is.EqualTo ("body_MyFormGridLabel"));

      var aspLabel = home.GetLabel().ByID ("body_MyAspLabel");
      Assert.That (aspLabel.Scope.Id, Is.EqualTo ("body_MyAspLabel"));

      var htmlLabel = home.GetLabel().ByID ("body_MyHtmlLabel");
      Assert.That (htmlLabel.Scope.Id, Is.EqualTo ("body_MyHtmlLabel"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var smartLabel = home.GetLabel().ByLocalID ("MySmartLabel");
      Assert.That (smartLabel.Scope.Id, Is.EqualTo ("body_MySmartLabel"));

      var formGridLabel = home.GetLabel().ByLocalID ("MyFormGridLabel");
      Assert.That (formGridLabel.Scope.Id, Is.EqualTo ("body_MyFormGridLabel"));

      var aspLabel = home.GetLabel().ByLocalID ("MyAspLabel");
      Assert.That (aspLabel.Scope.Id, Is.EqualTo ("body_MyAspLabel"));

      var htmlLabel = home.GetLabel().ByLocalID ("MyHtmlLabel");
      Assert.That (htmlLabel.Scope.Id, Is.EqualTo ("body_MyHtmlLabel"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var smartLabel = home.GetLabel().ByLocalID ("MySmartLabel");
      Assert.That (smartLabel.GetText(), Is.EqualTo ("MySmartLabelContent"));

      var formGridLabel = home.GetLabel().ByLocalID ("MyFormGridLabel");
      Assert.That (formGridLabel.GetText(), Is.EqualTo ("MyFormGridLabelContent"));

      var aspLabel = home.GetLabel().ByLocalID ("MyAspLabel");
      Assert.That (aspLabel.GetText(), Is.EqualTo ("MyAspLabelContent"));

      var htmlLabel = home.GetLabel().ByLocalID ("MyHtmlLabel");
      Assert.That (htmlLabel.GetText(), Is.EqualTo ("MyHtmlLabelContent"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("LabelTest.aspx");
    }
  }
}