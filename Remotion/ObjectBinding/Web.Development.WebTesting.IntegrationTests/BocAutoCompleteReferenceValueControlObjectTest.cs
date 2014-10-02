using System;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByID ("body_DataEditControl_PartnerField_Normal");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByIndex (2);
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal_AlternativeRendering"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().First();
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetAutoComplete().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC auto complete reference value.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByDisplayName ("Partner");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByDomainProperty ("Partner");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByDomainProperty ("Partner", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestText ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Disabled");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_NoAutoPostBack");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_NoCommandNoMenu");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("Invalid");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("B, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baLabel));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_NoAutoPostBack");
      bocAutoComplete.FillWith ("B, A"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("B, A", WaitFor.Nothing); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("D, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (daLabel));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (baLabel));
    }

    [Test]
    public void TestExecuteCommand ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.ExecuteCommand();

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);
    }

    private RemotionPageObject Start()
    {
      return Start ("BocAutoCompleteReferenceValue");
    }
  }
}