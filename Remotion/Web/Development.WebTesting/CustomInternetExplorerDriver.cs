using System;
using System.IO;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Custom <see cref="SeleniumWebDriver"/> implementation for <see cref="Browser.InternetExplorer"/>. The default implementation of Coypu does not
  /// set all <see cref="InternetExplorerOptions"/> and does not enable driver-internal logging.
  /// </summary>
  public class CustomInternetExplorerDriver : SeleniumWebDriver
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CustomInternetExplorerDriver));

    public CustomInternetExplorerDriver ()
        : base (CreateInternetExplorerDriver(), Browser.InternetExplorer)
    {
    }

    private static IWebDriver CreateInternetExplorerDriver ()
    {
      var driverService = InternetExplorerDriverService.CreateDefaultService();
      driverService.LogFile = GetLogFile();
      driverService.LoggingLevel = InternetExplorerDriverLogLevel.Info;

      return
          new InternetExplorerDriver (
              driverService,
              new InternetExplorerOptions
              {
                  EnableNativeEvents = true,
                  RequireWindowFocus = true,
                  EnablePersistentHover = false
              });
    }

    private static string GetLogFile ()
    {
      EnsureLogsDirectoryExists();

      // Note: unfortunately there is no append-mode for this log and we do not have enough context information to create a nice name.
      for (var i = 0;; ++i)
      {
        var fileName = string.Format ("InternetExplorerDriver{0}.log", i);
        var logFile = Path.Combine (WebTestingConfiguration.Current.LogsDirectory, fileName);

        if (File.Exists (logFile))
          continue;

        // Log file name in order to give the user the chance to correlate the log file to test executions.
        s_log.InfoFormat ("Internet explorer driver logs to: '{0}'.", fileName);
        return logFile;
      }
    }

    private static void EnsureLogsDirectoryExists ()
    {
      Directory.CreateDirectory (WebTestingConfiguration.Current.LogsDirectory);
    }
  }
}