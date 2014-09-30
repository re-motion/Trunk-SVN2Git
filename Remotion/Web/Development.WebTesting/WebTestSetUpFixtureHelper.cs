using System;
using JetBrains.Annotations;
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
    private static readonly ILog s_log = LogManager.GetLogger (typeof (WebTestSetUpFixtureHelper));

    private readonly IHostingStrategy _hostingStrategy;

    public WebTestSetUpFixtureHelper([NotNull] IHostingStrategy hostingStrategy)
    {
      ArgumentUtility.CheckNotNull ("hostingStrategy", hostingStrategy);
      
      _hostingStrategy = hostingStrategy;
    }

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
      _hostingStrategy.DeployAndStartWebApplication();
    }

    private void UnhostWebApplication ()
    {
      _hostingStrategy.StopAndUndeployWebApplication();
    }
  }
}