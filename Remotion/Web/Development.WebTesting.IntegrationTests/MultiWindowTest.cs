using System;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  public class MultiWindowTestPageObject : RemotionPageObject
  {
    public MultiWindowTestPageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    public RemotionPageObject Frame
    {
      get
      {
        var frameScope = Scope.FindFrame ("frame");
        return new RemotionPageObject (Context.CloneForFrame (frameScope));
      }
    }
  }

  [TestFixture]
  public class MultiWindowTest : IntegrationTest
  {
    [Test]
    public void TestMultiFrameActions ()
    {
      var home = Start();

      var mainLabel = home.GetLabel().ByID ("MainLabel");
      AssertPostBackSequenceNumber (mainLabel, 1);

      var frameLabel = home.Frame.GetLabel().ByID ("FrameLabel");
      AssertPostBackSequenceNumber (frameLabel, 1);

      var simplePostBackButton = home.GetWebButton().ByID ("SimplePostBack");
      simplePostBackButton.Click();
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);

      var loadFrameFunctionAsSubInFrameButton = home.GetWebButton().ByID ("LoadFrameFunctionAsSubInFrame");
      loadFrameFunctionAsSubInFrameButton.Click(Behavior.WaitForWxePostBackIn(home.Frame));
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 3);

      var loadFrameFunctionInFrameButton = home.GetWebButton().ByID ("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click(Behavior.WaitForWxeResetIn(home.Frame));
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 4);

      var simplePostBackButtonInFrameButton = home.Frame.GetWebButton().ByID ("SimplePostBack");
      simplePostBackButtonInFrameButton.Click();
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 4);

      var refreshMainUpdatePanelButton = home.Frame.GetWebButton().ByID ("RefreshMainUpdatePanel");
      refreshMainUpdatePanelButton.Click(Behavior.WaitForWxePostBackIn(home));
      AssertPostBackSequenceNumber (frameLabel, 3);
      AssertPostBackSequenceNumber (mainLabel, 5);

      var loadMainAutoRefreshingFrameFunctionInFrameButton = home.GetWebButton().ByID ("LoadMainAutoRefreshingFrameFunctionInFrame");
      loadMainAutoRefreshingFrameFunctionInFrameButton.Click(Behavior.WaitForWxeResetIn(home.Frame));
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 6);

      simplePostBackButtonInFrameButton.Click(new ActionBehavior().WaitFor(WaitFor.WxePostBack).WaitFor(WaitFor.WxePostBackIn(home)));
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 7);
    }

    [Test]
    public void TestMultiWindowActions()
    {
      var home = Start();

      var mainLabel = home.GetLabel().ByID ("MainLabel");
      AssertPostBackSequenceNumber (mainLabel, 1);

      var frameLabel = home.Frame.GetLabel().ByID ("FrameLabel");
      AssertPostBackSequenceNumber (frameLabel, 1);

      var loadWindowFunctionInNewWindowButton = home.GetWebButton().ByID ("LoadWindowFunctionInNewWindow");
      var window = loadWindowFunctionInNewWindowButton.Click().ExpectNewPopupWindow<RemotionPageObject> ("MyWindow");
      var windowLabel = window.GetLabel().ByID ("WindowLabel");
      AssertPostBackSequenceNumber (windowLabel, 1);
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);
      
      var simplePostBackButtonInWindowButton = window.GetWebButton().ByID ("SimplePostBack");
      simplePostBackButtonInWindowButton.Click();
      AssertPostBackSequenceNumber (windowLabel, 2);
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 2);

      var closeButton = window.GetWebButton().ByID ("Close");
      closeButton.Click (Behavior.WaitForWxePostBackIn (home).ClosesWindow());
      AssertPostBackSequenceNumber (frameLabel, 1);
      AssertPostBackSequenceNumber (mainLabel, 3);

      var loadWindowFunctionInNewWindowInFrameButton = home.Frame.GetWebButton().ByID ("LoadWindowFunctionInNewWindow");
      loadWindowFunctionInNewWindowInFrameButton.Click().ExpectNewPopupWindow<RemotionPageObject> ("MyWindow");
      AssertPostBackSequenceNumber (windowLabel, 1);
      AssertPostBackSequenceNumber (frameLabel, 2);
      AssertPostBackSequenceNumber (mainLabel, 3);

      var closeAndRefreshMainAsWellButton = window.GetWebButton().ByID ("CloseAndRefreshMainAsWell");
      closeAndRefreshMainAsWellButton.Click (
          new ActionBehavior().WaitFor (WaitFor.WxePostBackIn (home.Frame)).WaitFor (WaitFor.WxePostBackIn (home)).ClosesWindow());
      AssertPostBackSequenceNumber (frameLabel, 3);
      AssertPostBackSequenceNumber (mainLabel, 4);
    }

    private void AssertPostBackSequenceNumber (LabelControlObject label, int expectedPostBackSequenceNumber)
    {
      Assert.That (label.GetText(), Is.StringContaining (string.Format ("| {0} |", expectedPostBackSequenceNumber)));
    }

    private MultiWindowTestPageObject Start ()
    {
      var home = Start ("MultiWindowTest/Main.wxe");
      return new MultiWindowTestPageObject (home.Context);
    }
  }
}