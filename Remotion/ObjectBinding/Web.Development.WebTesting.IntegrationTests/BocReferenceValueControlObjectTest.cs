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
  public class BocReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByID ("body_DataEditControl_PartnerField_Normal");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByIndex (2);
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal_AlternativeRendering"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().First();
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetReferenceValue().Single();
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

      var bocReferenceValue = home.GetReferenceValue().ByDisplayName ("Partner");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByDomainProperty ("Partner");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByDomainProperty ("Partner", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Disabled");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_NoAutoPostBack");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_NoCommandNoMenu");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));
    }

    [Test]
    public void TestSelectOption ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (baLabel);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo(baLabel));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (1);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOptionByText ("D, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo(daLabel));
    }

    [Test]
    public void TestSelectOptionPostBack ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption ("==null==");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (baLabel);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baLabel));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_NoAutoPostBack");
      bocReferenceValue.SelectOption (baLabel); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (baLabel, Behavior.WaitFor (WaitFor.Nothing)); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (daLabel);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (daLabel));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (baLabel));
    }

    [Test]
    public void TestExecuteCommand ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      var dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.ClickItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.ClickItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.ClickItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.ClickItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));
    }

    private RemotionPageObject Start()
    {
      return Start ("BocReferenceValue");
    }
  }
}