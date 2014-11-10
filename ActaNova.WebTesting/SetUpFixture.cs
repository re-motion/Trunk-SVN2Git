using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private readonly WebTestSetUpFixtureHelper _setUpFixtureHelper = WebTestSetUpFixtureHelper.CreateFromConfiguration();

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