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
using System.Collections.Generic;
using Coypu;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="IDriver"/> interface.
  /// </summary>
  public static class DriverExtensions
  {
    private const string c_unknown = "unknown";

    /// <summary>
    /// Gets the name of the browser associated with <paramref name="driver"/> or a <see cref="string"/> indicating that it could not be determined.
    /// </summary>
    public static string GetBrowserName (this IDriver driver)
    {
      return driver.Native switch
      {
          ChromeDriver _ => "Chrome",
          EdgeDriver _ => "Edge",
          FirefoxDriver _ => "Firefox",
          _ => c_unknown
      };
    }

    /// <summary>
    /// Gets the version of the browser associated with <paramref name="driver"/> or a <see cref="string"/> indicating that it could not be determined.
    /// </summary>
    public static string GetBrowserVersion (this IDriver driver)
    {
      if (!(driver.Native is IHasCapabilities driverWithCapabilities))
        return c_unknown;

      return driver.Native switch
      {
          ChromeDriver _ => driverWithCapabilities.Capabilities.GetCapability ("version") as string ?? c_unknown,
          EdgeDriver _ => driverWithCapabilities.Capabilities.GetCapability ("version") as string ?? c_unknown,
          FirefoxDriver _ => driverWithCapabilities.Capabilities.GetCapability ("browserVersion") as string ?? c_unknown,
          _ => c_unknown
      };
    }

    /// <summary>
    /// Gets the name of the webdriver associated with <paramref name="driver"/> or a <see cref="string"/> indicating that it could not be determined.
    /// </summary>
    public static string GetWebdriverVersion (this IDriver driver)
    {
      if (!(driver.Native is IHasCapabilities driverWithCapabilities))
        return c_unknown;

      return driver.Native switch
      {
          ChromeDriver _ when driverWithCapabilities.Capabilities.GetCapability ("chrome") is Dictionary<string, object> capabilities
                              && capabilities.TryGetValue ("chromedriverVersion", out var driverVersion) => driverVersion as string ?? c_unknown,
          EdgeDriver _ when driverWithCapabilities.Capabilities.GetCapability ("msedge") is Dictionary<string, object> capabilities
                            && capabilities.TryGetValue ("msedgedriverVersion", out var driverVersion) => driverVersion as string ?? c_unknown,
          FirefoxDriver _ => driverWithCapabilities.Capabilities.GetCapability ("moz:geckodriverVersion") as string ?? c_unknown,
          _ => c_unknown
      };
    }
  }
}