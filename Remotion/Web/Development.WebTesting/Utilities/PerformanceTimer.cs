using System;
using System.Diagnostics;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utility class to measure the performance of code and log the results.
  /// </summary>
  public class PerformanceTimer : IDisposable
  {
    private readonly ILog _log;
    private readonly string _message;
    private readonly Stopwatch _stopwatch;

    public PerformanceTimer ([NotNull] ILog log, string message)
    {
      ArgumentUtility.CheckNotNull ("log", log);

      _log = log;
      _message = message;
      _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose ()
    {
      _stopwatch.Stop();
      _log.DebugFormat (_message + " [took: {0}]", _stopwatch.Elapsed);
    }
  }
}