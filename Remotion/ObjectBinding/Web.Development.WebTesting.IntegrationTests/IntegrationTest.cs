using System;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all integration tests.
  /// </summary>
  public abstract class IntegrationTest
  {
    private readonly WebTestHelper _webTestHelper = new WebTestHelper();

    [TestFixtureSetUp]
    public void IntegrationTestTestFixtureSetUp ()
    {
      _webTestHelper.OnFixtureSetUp();
    }

    [SetUp]
    public void IntegrationTestSetUp ()
    {
      _webTestHelper.OnSetUp (TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void IntegrationTestTearDown ()
    {
      var hasSucceeded = TestContext.CurrentContext.Result.Status != TestStatus.Failed;
      _webTestHelper.OnTearDown (hasSucceeded);
    }

    [TestFixtureTearDown]
    public void IntegrationTestTestFixtureTearDown ()
    {
      _webTestHelper.OnFixtureTearDown();
    }

    protected RemotionPageObject Start (string userControl)
    {
      var userControlUrl = string.Format ("Controls/{0}UserControl.ascx", userControl);

      var url = string.Format ("{0}ControlTest.wxe?UserControl={1}", _webTestHelper.Configuration.WebApplicationRoot, userControlUrl);
      _webTestHelper.MainBrowserSession.Visit (url);
      AcceptPossibleModalDialog();

      var context = _webTestHelper.CreateNewTestObjectContext();
      return new UnspecifiedPageObject (context).Expect<RemotionPageObject>();
    }

    protected IActionBehavior Behavior
    {
      // Todo RM-6297 @ MK: Property which returns a new object ... okay for better readability?
      get { return new ActionBehavior(); }
    }

    private void AcceptPossibleModalDialog ()
    {
      try
      {
        var context = _webTestHelper.CreateNewTestObjectContext();
        _webTestHelper.MainBrowserSession.AcceptModalDialogImmediatelyFixed (context);
      }
      catch (NoAlertPresentException)
      {
        // It's okay.
      }
    }
  }
}