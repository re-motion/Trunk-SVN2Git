using System;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Executes a given <see cref="Func{TReturnType}"/> repeatedly (using the given retry interval) until no exception is thrown during execution or
  /// until the given timeout has been reached (in which case the final exception is rethrown).
  /// </summary>
  public class RetryUntilTimeout<TReturnType>
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (RetryUntilTimeout<TReturnType>));

    private readonly Func<TReturnType> _func;
    private readonly TimeSpan _timeout;
    private readonly TimeSpan _retryInterval;

    public RetryUntilTimeout (Func<TReturnType> func, TimeSpan timeout, TimeSpan retryInterval)
    {
      _func = func;
      _timeout = timeout;
      _retryInterval = retryInterval;
    }

    public TReturnType Run ()
    {
      var stopwatch = Stopwatch.StartNew();

      do
      {
        try
        {
          return _func();
        }
        catch (Exception ex)
        {
          if (stopwatch.ElapsedMilliseconds < _timeout.TotalMilliseconds)
          {
            s_log.Debug ("RetryUntilTimeout failed with " + ex.GetType().Name + " - trying again.");
            Thread.Sleep (_retryInterval);
          }
          else
          {
            s_log.Warn ("RetryUntilTimeout failed with " + ex.GetType().Name + " - timeout elapsed, failing.");
            throw;
          }
        }
      } while (true);
    }
  }
}