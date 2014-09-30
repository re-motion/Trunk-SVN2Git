using System;
using System.Configuration;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web test set up fixtures. Initializes log4net, ensures that the screenshot directory exists and most importantly hosts the
  /// configured web application in its own IIS express instance.
  /// </summary>
  public class WebTestSetUpFixtureHelper
  {
    // Todo RM-6297: IIS Express hosting should be optional.

    private static readonly ILog s_log = LogManager.GetLogger (typeof (WebTestSetUpFixtureHelper));

    private IisExpressProcessWrapper _webApplicationHost;

    /// <summary>
    /// One-time SetUp method for all web tests.
    /// </summary>
    public void OnSetUp ()
    {
      SetUpLog4net();
      EnsureScreenshotDirectoryExists();
      DetermineScreenSizeForScreenshots();
      HostWebApplication();
    }

    /// <summary>
    /// One-time TearDown method for all web tests.
    /// </summary>
    public void OnTearDown ()
    {
      UnhostWebApplication();
    }

    private void SetUpLog4net ()
    {
      XmlConfigurator.Configure();
    }

    private void EnsureScreenshotDirectoryExists ()
    {
      // Todo RM-6297: EnsureScreenshotDirectoryExists.
    }

    private void DetermineScreenSizeForScreenshots ()
    {
      // Todo RM-6297: DetermineScreenSizeForScreenshots.
    }

    private void HostWebApplication ()
    {
      var webTestConfiguration = (WebTestConfiguration) ConfigurationManager.GetSection ("webTestConfiguration");
      Assertion.IsNotNull(webTestConfiguration,"Configuration section 'webTestConfiguration' missing.");

      var absoluteWebTestApplicationPath = Path.GetFullPath (webTestConfiguration.WebApplicationPath);
      _webApplicationHost = new IisExpressProcessWrapper (absoluteWebTestApplicationPath, webTestConfiguration.WebApplicationPort);

      var iisExpressThread = new Thread (() => _webApplicationHost.Run()) { IsBackground = true };
      iisExpressThread.Start();
    }

    private void UnhostWebApplication ()
    {
      if (_webApplicationHost != null)
        _webApplicationHost.Dispose();
    }
  }
}