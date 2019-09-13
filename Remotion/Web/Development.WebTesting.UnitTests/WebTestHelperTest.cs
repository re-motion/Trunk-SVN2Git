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
    private readonly TimeSpan _configCommandTimeout = TimeSpan.FromSeconds ((13 * 60) + 37);
    private readonly TimeSpan _configSearchTimeout = TimeSpan.FromSeconds (30);
    private readonly TimeSpan _configRetryInterval = TimeSpan.FromMilliseconds (25);

    [Test]
    public void CreateNewBrowserSession_DefaultValue ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var defaultCommandTimeout = webTestHelper.TestInfrastructureConfiguration.DriverConfiguration.CommandTimeout;
      var browserFactoryMock = webTestHelper.BrowserConfiguration.BrowserFactory;
      browserFactoryMock
          .Expect (_ => _.CreateBrowser (Arg<DriverConfiguration>.Matches (configuration => configuration.CommandTimeout == defaultCommandTimeout)))
          .Return (null);

      webTestHelper.CreateNewBrowserSession (false);

      var createBrowserArguments = browserFactoryMock.GetArgumentsForCallsMadeOn (_ => _.CreateBrowser (null));
      var driverConfigurationArgument = (DriverConfiguration) createBrowserArguments[0][0];
      Assert.That (driverConfigurationArgument.CommandTimeout, Is.EqualTo (_configCommandTimeout));
      Assert.That (driverConfigurationArgument.SearchTimeout, Is.EqualTo (_configSearchTimeout));
      Assert.That (driverConfigurationArgument.RetryInterval, Is.EqualTo (_configRetryInterval));
    }

    [Test]
    public void CreateNewBrowserSession_CommandTimeoutOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { CommandTimeout = TimeSpan.FromMinutes (5) };
      var browserFactoryMock = webTestHelper.BrowserConfiguration.BrowserFactory;
      browserFactoryMock
          .Expect (_ => _.CreateBrowser (Arg<DriverConfiguration>.Is.Anything))
          .Return (null);

      webTestHelper.CreateNewBrowserSession (false, configurationOverride);

      var createBrowserArguments = browserFactoryMock.GetArgumentsForCallsMadeOn (_ => _.CreateBrowser (null));
      var driverConfigurationArgument = (DriverConfiguration) createBrowserArguments[0][0];
      Assert.That (driverConfigurationArgument.CommandTimeout, Is.EqualTo (configurationOverride.CommandTimeout));
      Assert.That (driverConfigurationArgument.SearchTimeout, Is.EqualTo (_configSearchTimeout));
      Assert.That (driverConfigurationArgument.RetryInterval, Is.EqualTo (_configRetryInterval));
    }

    [Test]
    public void CreateNewBrowserSession_SearchTimeoutOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { SearchTimeout = TimeSpan.FromMinutes (5) };
      var browserFactoryMock = webTestHelper.BrowserConfiguration.BrowserFactory;
      browserFactoryMock
          .Expect (_ => _.CreateBrowser (Arg<DriverConfiguration>.Is.Anything))
          .Return (null);

      webTestHelper.CreateNewBrowserSession (false, configurationOverride);

      var createBrowserArguments = browserFactoryMock.GetArgumentsForCallsMadeOn (_ => _.CreateBrowser (null));
      var driverConfigurationArgument = (DriverConfiguration) createBrowserArguments[0][0];
      Assert.That (driverConfigurationArgument.CommandTimeout, Is.EqualTo (_configCommandTimeout));
      Assert.That (driverConfigurationArgument.SearchTimeout, Is.EqualTo (configurationOverride.SearchTimeout));
      Assert.That (driverConfigurationArgument.RetryInterval, Is.EqualTo (_configRetryInterval));
    }

    [Test]
    public void CreateNewBrowserSession_RetryIntervalOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { RetryInterval = TimeSpan.FromMinutes (5) };
      var browserFactoryMock = webTestHelper.BrowserConfiguration.BrowserFactory;
      browserFactoryMock
          .Expect (_ => _.CreateBrowser (Arg<DriverConfiguration>.Is.Anything))
          .Return (null);

      webTestHelper.CreateNewBrowserSession (false, configurationOverride);

      var createBrowserArguments = browserFactoryMock.GetArgumentsForCallsMadeOn (_ => _.CreateBrowser (null));
      var driverConfigurationArgument = (DriverConfiguration) createBrowserArguments[0][0];
      Assert.That (driverConfigurationArgument.CommandTimeout, Is.EqualTo (_configCommandTimeout));
      Assert.That (driverConfigurationArgument.SearchTimeout, Is.EqualTo (_configSearchTimeout));
      Assert.That (driverConfigurationArgument.RetryInterval, Is.EqualTo (configurationOverride.RetryInterval));
    }

    private class TestWebTestConfigurationFactory : WebTestConfigurationFactory
    {
      protected override IChromeConfiguration CreateChromeConfiguration (WebTestConfigurationSection configSettings)
      {
        var browserFactoryMock = MockRepository.GenerateMock<IBrowserFactory>();

        var chromeConfigurationStub = MockRepository.GenerateStub<IChromeConfiguration>();
        chromeConfigurationStub
            .Stub (_ => _.BrowserFactory)
            .Return (browserFactoryMock);

        return chromeConfigurationStub;
      }
    }
  }
}