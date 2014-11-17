using System;
using ActaNova.WebTesting.PageObjects;
using Coypu;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Configuration;

namespace ActaNova.WebTesting
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
    }

    [TearDown]
    public void ActaNovaWebTestBaseTearDown ()
    {
      // Todo RM-6297: Remove as soon as the ActaNova language problem has been solved and the default Chrome driver can be used again (if okay for IE).
      _webTestHelper.MainBrowserSession.ClearCookies();

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
      const string defaultQueryString = "?debugLoginUser=mm&debugDmsDownLevel=true&debugCulture=DE-AT";
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
      return _webTestHelper.CreateInitialPageObject<ActaNovaMainPageObject>(_webTestHelper.MainBrowserSession);
    }

    protected BrowserSession CreateNewBrowser ()
    {
      return _webTestHelper.CreateNewBrowserSession ();
    }

    protected TPageObject StartAgain<TPageObject> ([NotNull] BrowserSession browser, [NotNull] string url)
      where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNullOrEmpty ("url", url);

      browser.Visit (url);
      return _webTestHelper.CreateInitialPageObject<TPageObject>(browser);
    }
  }
}