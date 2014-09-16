using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
     private readonly WebTestSetUpFixtureHelper _setUpFixtureHelper = new WebTestSetUpFixtureHelper();

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