﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories.Chrome
{
  /// <summary>
  /// Responsible for creating a new Chrome browser, configured based on <see cref="IChromeConfiguration"/> and <see cref="ITestInfrastructureConfiguration"/>.
  /// </summary>
  public class ChromeBrowserFactory : IBrowserFactory
  {
    private readonly IChromeConfiguration _chromeConfiguration;

    public ChromeBrowserFactory ([NotNull] IChromeConfiguration chromeConfiguration)
    {
      ArgumentUtility.CheckNotNull ("chromeConfiguration", chromeConfiguration);

      _chromeConfiguration = chromeConfiguration;
    }

    public BrowserSession CreateBrowser (ITestInfrastructureConfiguration testInfrastructureConfiguration)
    {
      ArgumentUtility.CheckNotNull ("testInfrastructureConfiguration", testInfrastructureConfiguration);

      var sessionConfiguration = CreateSessionConfiguration (testInfrastructureConfiguration);
      var driver = CreateChromeDriver();

      return new BrowserSession (sessionConfiguration, new CustomSeleniumWebDriver (driver, Browser.Chrome));
    }

    private SessionConfiguration CreateSessionConfiguration (ITestInfrastructureConfiguration testInfrastructureConfiguration)
    {
      return new SessionConfiguration
             {
                 Browser = Browser.Chrome,
                 RetryInterval = testInfrastructureConfiguration.RetryInterval,
                 Timeout = testInfrastructureConfiguration.SearchTimeout,
                 ConsiderInvisibleElements = WebTestingConstants.ShouldConsiderInvisibleElements,
                 Match = WebTestingConstants.DefaultMatchStrategy,
                 TextPrecision = WebTestingConstants.DefaultTextPrecision,
                 Driver = typeof (CustomSeleniumWebDriver)
             };
    }

    private ChromeDriver CreateChromeDriver ()
    {
      var driverService = CreateChromeDriverService();
      var chromeOptions = _chromeConfiguration.CreateChromeOptions();

      var driver = new ChromeDriver (driverService, chromeOptions);

      CloseWelcomeTab (driver);

      return driver;
    }

    private ChromeDriverService CreateChromeDriverService ()
    {
      var driverService = ChromeDriverService.CreateDefaultService();

      driverService.EnableVerboseLogging = false;
      driverService.LogPath = WebDriverLogUtility.CreateLogFile (_chromeConfiguration.LogsDirectory, _chromeConfiguration.BrowserName);

      return driverService;
    }

    private void CloseWelcomeTab (ChromeDriver driver)
    {
      //The argument "--no-first-run" is not working correctly with Chrome v53. 
      //It does not create any first run artifacts, but does not block the first run "Welcome" tab.
      //Reevulate in later Chrome versions, as just relying on this command line argument is more clean than closing the tab after creating the ChromeDriver.
      
      //When an empty CustomUserDirectory is used, Chrome launches a first-time "Welcome" Tab. 
      //This has to be closed before continuing with the tests
      if (driver.WindowHandles.Count == 2)
      {
        driver.SwitchTo().Window (driver.WindowHandles[1]);
        driver.Close();
        driver.SwitchTo().Window (driver.WindowHandles[0]);
      }
    }
  }
}