using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class InfrastructureTests : IntegrationTest
  {
    [Test]
    [TestCase ("! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9")]
    [TestCase ("! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9! \" § $ % & / ( ) =  ? ² ³ { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9")]
    //[TestCase("°")] // Todo RM-6297: Does not work in Chrome with de_AT keyboard, see https://code.google.com/p/chromedriver/issues/detail?id=932
    //[TestCase("^")] // Todo RM-6297: Does not work in IE with de_AT keyboard, see http://stackoverflow.com/questions/26357191/
    public void TestCoypuElementScopeFillInWithAndSendKeysExtensions_FillWithAndWait (string input)
    {
      // Todo RM-6297: Fix problems on TeamCity with FillWithFixed.
      if (WebTestingFrameworkConfiguration.Current.BrowserIsInternetExplorer())
        Assert.Ignore("Currently ignored until TeamCity-related probelms with FillWithFixed are fixed.");

      var home = Start();

      var textBox = home.GetTextBox().ByLocalID("MyTextBox");

      textBox.FillWith (input, FinishInput.WithTab);

      Assert.That (home.Scope.FindId ("wxePostBackSequenceNumberField").Value, Is.EqualTo ("3"));
      Assert.That (textBox.GetText(), Is.EqualTo (input));
    }

    private RemotionPageObject Start ()
    {
      return Start ("InfrastructureTests.wxe");
    }
  }
}