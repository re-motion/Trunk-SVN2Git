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
    [TestCase ("! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ A Z a z 0 1 8 9")]
    [TestCase ("! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ A Z a z 0 1 8 9! \" § $ % & / ( ) =  ? ² ³ { [ ] } \\ + * ~ ' # @ A Z a z 0 1 8 9")]
    //[TestCase("°")] // Todo RM-6297: Does not work in Chrome with de_AT keyboard, see https://code.google.com/p/chromedriver/issues/detail?id=932
    //[TestCase("^")] // Todo RM-6297: Does not work in IE with de_AT keyboard, see http://stackoverflow.com/questions/26357191/
    public void TestFillWithAndWait_FromCoypuWaitingElementScopeExtensions (string input)
    {
      var home = Start();

      // Todo RM-6297: Replace with TextBox (HtmlInputTextControlObject or TextBoxControlObject?) control as soon as implemented.
      var textBox = home.Scope.FindId ("body_MyTextBox");

      textBox.FillWithAndWait (input, Then.TabAway, home.Context, Behavior.WaitFor (WaitFor.WxePostBack));

      Assert.That (textBox.Value, Is.EqualTo (input));
      Assert.That (home.Scope.FindId ("wxePostBackSequenceNumberField").Value, Is.EqualTo ("3"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("InfrastructureTests.wxe");
    }
  }
}