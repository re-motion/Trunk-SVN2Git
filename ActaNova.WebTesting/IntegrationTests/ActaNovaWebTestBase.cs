using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Configuration;

namespace ActaNova.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all ActaNova web tests.
  /// </summary>
  public abstract class ActaNovaWebTestBase
  {
    private readonly WebTestHelper _webTestHelper = WebTestHelper.CreateFromConfiguration();

    [TestFixtureSetUp]
    public void ActaNovaWebTestBaseTestFixtureSetUp ()
    {
      _webTestHelper.OnFixtureSetUp();
    }

    [SetUp]
    public void ActaNovaWebTestBaseSetUp ()
    {
      _webTestHelper.OnSetUp (GetType().Name + "_" + TestContext.CurrentContext.Test.Name);
      _webTestHelper.MainBrowserSession.ClearCookies();
    }

    [TearDown]
    public void ActaNovaWebTestBaseTearDown ()
    {
      var hasSucceeded = TestContext.CurrentContext.Result.Status != TestStatus.Failed;
      _webTestHelper.OnTearDown (hasSucceeded);
    }

    [TestFixtureTearDown]
    public void ActaNovaWebTestBaseTestFixtureTearDown ()
    {
      _webTestHelper.OnFixtureTearDown();
    }

    protected ActaNovaMainPageObject Start ()
    {
      const string defaultQueryString = "?debugLoginUser=mm&debugDmsDownLevel=true";
      return Start (defaultQueryString);
    }

    protected ActaNovaMainPageObject Start ([NotNull] string queryString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryString", queryString);

      var url = WebTestingConfiguration.Current.WebApplicationRoot + queryString;

      // Visit blank page in order to trigger all WxeAbort calls: this prevents that the initial load of the right frame triggers a main frame update.
      _webTestHelper.MainBrowserSession.Visit ("about:blank");
      _webTestHelper.AcceptPossibleModalDialog();

      _webTestHelper.MainBrowserSession.Visit (url);
      return _webTestHelper.CreateInitialPageObject<ActaNovaMainPageObject>();
    }
  }
}