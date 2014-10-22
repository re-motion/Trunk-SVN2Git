using System;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

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

    public WebTestSetUpFixtureHelper ([NotNull] IHostingStrategy hostingStrategy)
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

    private void HostWebApplication ()
    {
      using (new PerformanceTimer (s_log, "Hosted web application."))
      {
        _hostingStrategy.DeployAndStartWebApplication();
      }
    }

    private void UnhostWebApplication ()
    {
      _hostingStrategy.StopAndUndeployWebApplication();
    }
  }
}