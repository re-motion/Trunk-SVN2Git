using System;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Hosts the web application using IIS Express.
  /// </summary>
  public class IisExpressHostingStrategy : IHostingStrategy
  {
    private readonly IisExpressProcessWrapper _webApplicationHost;

    /// <param name="webApplicationPath">Absolute or relative path to the web application source.</param>
    /// <param name="port">Port to be used.</param>
    public IisExpressHostingStrategy ([NotNull] string webApplicationPath, int port)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("webApplicationPath", webApplicationPath);

      var absoluteWebApplicationPath = Path.GetFullPath (webApplicationPath);
      _webApplicationHost = new IisExpressProcessWrapper (absoluteWebApplicationPath, port);
    }

    public void DeployAndStartWebApplication ()
    {
      var iisExpressThread = new Thread (() => _webApplicationHost.Run()) { IsBackground = true };
      iisExpressThread.Start();
    }

    public void StopAndUndeployWebApplication ()
    {
      if (_webApplicationHost != null)
        _webApplicationHost.Dispose();
    }
  }
}