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
using System.IO;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Custom <see cref="SeleniumWebDriver"/> implementation for <see cref="Browser.Chrome"/>. The default implementation uses a temporary profile
  /// which sets the browser AcceptLanguage to English, which is not suitable for ActaNova tests.
  /// </summary>
  public class CustomChromeDriver : SeleniumWebDriver
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CustomChromeDriver));

    public CustomChromeDriver ()
        : base (CreateChromeDriver(), Browser.Chrome)
    {
    }

    private static IWebDriver CreateChromeDriver ()
    {
      var driverService = ChromeDriverService.CreateDefaultService();

      var chromeOptions = new ChromeOptions();
      var userDataDirPath = GetUserDataDirPath();
      s_log.InfoFormat ("Chrome driver user-data-dir='{0}'.", userDataDirPath);
      chromeOptions.AddArgument (string.Format ("user-data-dir={0}", userDataDirPath));
      
      return new ChromeDriver (
          driverService,
          chromeOptions);
    }

    private static string GetUserDataDirPath ()
    {
      var path = Path.Combine (Path.GetDirectoryName(typeof (CustomChromeDriver).Assembly.CodeBase), "..", "..", "ChromeUserDataDir");
      path = path.Substring ("file:/".Length);
      return Path.GetFullPath (path);
    }
  }
}