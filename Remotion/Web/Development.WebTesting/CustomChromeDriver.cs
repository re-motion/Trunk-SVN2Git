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