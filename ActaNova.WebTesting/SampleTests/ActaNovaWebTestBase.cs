using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.SampleTests
{
  /// <summary>
  /// Base class for all ActaNova web tests.
  /// </summary>
  public abstract class ActaNovaWebTestBase
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

    protected ActaNovaMainPageObject Start ()
    {
      var url = _webTestHelper.Configuration.WebApplicationRoot;
      _webTestHelper.MainBrowserSession.Visit (url);

      var context = TestObjectContext.New (_webTestHelper.Configuration, _webTestHelper.MainBrowserSession);
      return new UnspecifiedPageObject (context).Expect<ActaNovaMainPageObject>();
    }
  }
}