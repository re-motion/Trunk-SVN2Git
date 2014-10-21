﻿using System;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Custom <see cref="SeleniumWebDriver"/> implementation for <see cref="Browser.InternetExplorer"/>. The default implementation of Coypu does not
  /// set all <see cref="InternetExplorerOptions"/> and does not enable driver-internal logging.
  /// </summary>
  public class CustomInternetExplorerDriver : SeleniumWebDriver
  {
    public CustomInternetExplorerDriver ()
        : base (CreateInternetExplorerDriver(), Browser.InternetExplorer)
    {
    }

    private static IWebDriver CreateInternetExplorerDriver ()
    {
      var driverService = InternetExplorerDriverService.CreateDefaultService();
      driverService.LogFile = "InternetExplorerDriverService.log"; // Todo RM-6297: obtain log path from App.config?
      driverService.LoggingLevel = InternetExplorerDriverLogLevel.Info;

      return
          new InternetExplorerDriver (
              driverService,
              new InternetExplorerOptions
              {
                  EnableNativeEvents = true,
                  RequireWindowFocus = true,
                  EnablePersistentHover = false,
                  IntroduceInstabilityByIgnoringProtectedModeSettings = true
              });
    }
  }
}