using System;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocEnumValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByID ("body_DataEditControl_MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));

      var listBoxBocEnumValue = home.GetEnumValue().ByID ("body_DataEditControl_MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_ListBoxNormal"));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByID ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByIndex (2);
      Assert.That (dropDownListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListReadOnly"));

      var listBoxBocEnumValue = home.GetEnumValue().ByIndex (5);
      Assert.That (listBoxBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_ListBoxNormal"));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByIndex (9);
      Assert.That (radioButtonListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_ListBoxNormal"));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue().First();
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetEnumValue().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC enum value.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue().ByDisplayName ("MarriageStatus");
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue().ByDomainProperty ("MarriageStatus");
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue()
          .ByDomainProperty ("MarriageStatus", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("BocEnumValue");
    }
  }
}