using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
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
      _webTestHelper.OnSetUp (GetType().Name + "_" + TestContext.CurrentContext.Test.Name);
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

    protected RemotionPageObject Start (string page)
    {
      var url = WebTestConfiguration.Current.WebApplicationRoot + page;
      _webTestHelper.MainBrowserSession.Visit (url);

      var context = _webTestHelper.CreateNewTestObjectContext();
      return new UnspecifiedPageObject (context).Expect<RemotionPageObject>();
    }

    protected IActionBehavior Behavior
    {
      // Note: property exists for "syntactical sugar" only, therefore returning a new object in the get accessor is okay.
      get { return new ActionBehavior(); }
    }
  }
}