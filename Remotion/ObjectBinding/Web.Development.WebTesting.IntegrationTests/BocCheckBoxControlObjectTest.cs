using System;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocCheckBoxControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByID ("body_DataEditControl_DeceasedField_Normal");
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByIndex (2);
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByLocalID ("DeceasedField_Normal");
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().First();
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetCheckBox().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC check box.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByDisplayName ("Deceased");
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByDomainProperty ("Deceased");
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByDomainProperty ("Deceased", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocCheckBox.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestState ()
    {
      var home = Start();

      var bocCheckBox = home.GetCheckBox().ByLocalID ("DeceasedField_Normal");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));

      bocCheckBox = home.GetCheckBox().ByLocalID ("DeceasedField_ReadOnly");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));

      bocCheckBox = home.GetCheckBox().ByLocalID ("DeceasedField_Disabled");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));

      bocCheckBox = home.GetCheckBox().ByLocalID ("DeceasedField_NoAutoPostBack");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));
    }

    [Test]
    public void TestSetTo ()
    {
      var home = Start();

      var normalBocBooleanValue = home.GetCheckBox().ByLocalID ("DeceasedField_Normal");
      var noAutoPostBackBocBooleanValue = home.GetCheckBox().ByLocalID ("DeceasedField_NoAutoPostBack");

      normalBocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("True"));

      noAutoPostBackBocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("False"));

      normalBocBooleanValue.SetTo (true, Continue.Immediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("False"));

      normalBocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("False"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("True"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("BocCheckBox");
    }
  }
}