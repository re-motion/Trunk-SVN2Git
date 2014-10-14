﻿using System;
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

    protected RemotionPageObject Start (string page)
    {
      var url = _webTestHelper.Configuration.WebApplicationRoot + page;
      _webTestHelper.MainBrowserSession.Visit (url);

      var context = TestObjectContext.New (_webTestHelper.Configuration, _webTestHelper.MainBrowserSession);
      return new UnspecifiedPageObject (context).Expect<RemotionPageObject>();
    }

    protected IActionBehavior Behavior
    {
      // Todo RM-6297: Property which returns a new object...for better readability.
      get { return new ActionBehavior(); }
    }
  }
}