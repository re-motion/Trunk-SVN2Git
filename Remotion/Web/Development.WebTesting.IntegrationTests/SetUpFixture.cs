using System;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private readonly WebTestSetUpFixtureHelper _setUpFixtureHelper = new WebTestSetUpFixtureHelper (
        new IisExpressAppConfigConfiguredHostingStrategy());

    [SetUp]
    public void SetUp ()
    {
      _setUpFixtureHelper.OnSetUp();
    }

    [TearDown]
    public void TearDown ()
    {
      _setUpFixtureHelper.OnTearDown();
    }
  }
}