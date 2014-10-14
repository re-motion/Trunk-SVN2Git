using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class InfrastructureTests : IntegrationTest
  {
    [Test]
    public void TestFillWithAndWait_FromCoypuWaitingElementScopeExtensions ()
    {
      const string inputWithSpecialCharacters = "! \" § $ % & / ( ) =  ? ² ³ { [ ] } \\ + * ~ ' # @ A Z a z 0 1 8 9";

      var home = Start();

      // Todo RM-6297: Replace with TextBox (HtmlInputTextControlObject or TextBoxControlObject?) control as soon as implemented.
      var textBox = home.Scope.FindId ("body_MyTextBox");

      textBox.FillWithAndWait (inputWithSpecialCharacters, Then.TabAway, home.Context, Behavior.WaitFor (WaitFor.WxePostBack));

      Assert.That (textBox.Value, Is.EqualTo (inputWithSpecialCharacters));
      Assert.That (home.Scope.FindId ("wxePostBackSequenceNumberField").Value, Is.EqualTo ("3"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("InfrastructureTests.wxe");
    }
  }
}