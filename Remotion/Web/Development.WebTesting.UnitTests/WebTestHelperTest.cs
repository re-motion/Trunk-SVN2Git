// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;
using Rhino.Mocks;

namespace Remotion.Web.Development.WebTesting.UnitTests
{
  [TestFixture]
  public class WebTestHelperTest
  {
    [Test]
    public void CreateNewBrowserSession_DefaultValue ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var defaultCommandTimeout = webTestHelper.TestInfrastructureConfiguration.DriverConfiguration.CommandTimeout;
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;
      browserFactoryStub
          .Expect (_ => _.CreateBrowser (Arg<DriverConfiguration>.Matches (configuration => configuration.CommandTimeout == defaultCommandTimeout)))
          .Return (null);

      webTestHelper.CreateNewBrowserSession (false);

      browserFactoryStub.VerifyAllExpectations();
    }

    [Test]
    public void CreateNewBrowserSession_Override ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { CommandTimeout = TimeSpan.FromMinutes (5) };
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;
      browserFactoryStub
          .Expect (_ => _.CreateBrowser (Arg<DriverConfiguration>.Matches (configuration => configuration.CommandTimeout == configurationOverride.CommandTimeout)))
          .Return (null);

      webTestHelper.CreateNewBrowserSession (false, configurationOverride);

      browserFactoryStub.VerifyAllExpectations();
    }

    private class TestWebTestConfigurationFactory : WebTestConfigurationFactory
    {
      protected override IChromeConfiguration CreateChromeConfiguration (WebTestConfigurationSection configSettings)
      {
        var browserFactoryStub = MockRepository.GenerateMock<IBrowserFactory>();

        var chromeConfigurationStub = MockRepository.GenerateStub<IChromeConfiguration>();
        chromeConfigurationStub
            .Stub (_ => _.BrowserFactory)
            .Return (browserFactoryStub);

        return chromeConfigurationStub;
      }
    }
  }
}