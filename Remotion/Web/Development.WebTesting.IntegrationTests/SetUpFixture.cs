﻿using System;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private readonly WebTestSetUpFixtureHelper _setUpFixtureHelper = new WebTestSetUpFixtureHelper (WebTestingFrameworkConfiguration.Current.GetHostingStrategy());

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