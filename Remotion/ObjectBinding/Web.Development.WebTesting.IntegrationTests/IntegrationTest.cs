using System;
using NUnit.Framework;
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

    public RemotionPageObject Start ()
    {
      var browser = _webTestHelper.MainBrowserSession;

      var context = TestObjectContext.New (browser);

      //return new UnspecifiedPageObject (context).ExpectPage(); // Todo RM-6297: ExpectPage should be specific to? Re-motion? Definitely not ActaNova.
      return new RemotionPageObject (context);
    }
  }
}