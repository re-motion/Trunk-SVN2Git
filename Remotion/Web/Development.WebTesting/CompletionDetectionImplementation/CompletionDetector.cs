using System;
using System.Collections.Generic;
using System.Threading;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Implementation class for <see cref="ICompletionDetection"/>, <see cref="IAdvancedCompletionDetection"/> and <see cref="ICompletionDetector"/>.
  /// </summary>
  internal class CompletionDetector : ICompletionDetection, IAdvancedCompletionDetection, ICompletionDetector
  {
    /// <summary>
    /// Returns a unique ID for an <see cref="CompletionDetector"/> (used for debug output purposes).
    /// </summary>
    private static class CompletionDetectorSequenceNumberGenerator
    {
      private static int s_counter = 0;

      public static int GetNextSequenceNumber ()
      {
        return Interlocked.Increment (ref s_counter);
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (CompletionDetector));

    private readonly List<ICompletionDetectionStrategy> _completionDetectionStrategies;
    private readonly List<Action<WebTestObjectContext>> _additionallyRequiredActions;
    private bool _useParentContext;

    private int _debugOutputID;
    private Dictionary<ICompletionDetectionStrategy, object> _completionDetectionStrategyStates;

    public CompletionDetector ()
    {
      _completionDetectionStrategies = new List<ICompletionDetectionStrategy>();
      _additionallyRequiredActions = new List<Action<WebTestObjectContext>>();
      _useParentContext = false;
    }

    public ICompletionDetector Build ()
    {
      return this;
    }

    public IAdvancedCompletionDetection And (ICompletionDetectionStrategy completionDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("completionDetectionStrategy", completionDetectionStrategy);

      _completionDetectionStrategies.Add (completionDetectionStrategy);
      return this;
    }

    public IAdvancedCompletionDetection AndWindowHasClosed ()
    {
      _useParentContext = true;
      return this;
    }

    public IAdvancedCompletionDetection AndModalDialogHasBeenAccepted ()
    {
      _additionallyRequiredActions.Add (
          ctx =>
          {
            Debug ("Accepting modal browser dialog.");
            ctx.Browser.AcceptModalDialogFixed();
          });

      return this;
    }

    public IAdvancedCompletionDetection AndModalDialogHasBeenCanceled ()
    {
      _additionallyRequiredActions.Add (
          ctx =>
          {
            Debug ("Canceling modal browser dialog.");
            ctx.Browser.CancelModalDialogFixed();
          });

      return this;
    }

    public void PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _debugOutputID = CompletionDetectorSequenceNumberGenerator.GetNextSequenceNumber();
      Debug ("Started.");
      Debug ("Collecting state for completion detection.");

      var actualContext = DetermineContextForCompletionDetectionStrategies (context);
      _completionDetectionStrategyStates = new Dictionary<ICompletionDetectionStrategy, object>();
      foreach (var completionDetectionStrategy in _completionDetectionStrategies)
      {
        var state = completionDetectionStrategy.PrepareWaitForCompletion (actualContext);
        _completionDetectionStrategyStates.Add (completionDetectionStrategy, state);
      }
    }

    public void WaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      Debug ("Waiting for completion...");

      ExecuteAdditionallyRequiredActions (context);

      var actualContext = DetermineContextForCompletionDetectionStrategies (context);
      foreach (var completionDetectionStrategy in _completionDetectionStrategies)
      {
        var state = _completionDetectionStrategyStates[completionDetectionStrategy];
        completionDetectionStrategy.WaitForCompletion (actualContext, state);
      }

      Debug ("Finished.");
    }

    private void ExecuteAdditionallyRequiredActions (PageObjectContext context)
    {
      foreach (var action in _additionallyRequiredActions)
        action (context);
    }

    private PageObjectContext DetermineContextForCompletionDetectionStrategies (PageObjectContext context)
    {
      if (_useParentContext)
      {
        Debug ("Using parent context for completion detection.");
        return context.ParentContext;
      }

      return context;
    }

    private void Debug (string message)
    {
      s_log.DebugFormat ("Action {0}: {1}", _debugOutputID, message);
    }
  }
}