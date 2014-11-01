using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Default implementation for <see cref="ICompletionDetector"/>.
  /// </summary>
  internal class CompletionDetector : ICompletionDetector
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

    private readonly int _debugOutputID;
    private readonly List<ICompletionDetectionStrategy> _completionDetectionStrategies;
    private readonly Dictionary<ICompletionDetectionStrategy, object> _completionDetectionStrategyStates;
    private readonly List<Action<CompletionDetector, WebTestObjectContext>> _additionallyRequiredActions;
    private readonly bool _useParentContext;

    private bool _prepareWaitForCompletionExecuted;
    private bool _waitForCompletionExecuted;

    public CompletionDetector (
        [NotNull] IEnumerable<ICompletionDetectionStrategy> completionDetectionStrategies,
        [NotNull] IEnumerable<Action<CompletionDetector, WebTestObjectContext>> additionallyRequiredActions,
        bool useParentContext)
    {
      _debugOutputID = CompletionDetectorSequenceNumberGenerator.GetNextSequenceNumber();
      _completionDetectionStrategies = new List<ICompletionDetectionStrategy> (completionDetectionStrategies);
      _completionDetectionStrategyStates = new Dictionary<ICompletionDetectionStrategy, object>();
      _additionallyRequiredActions = new List<Action<CompletionDetector, WebTestObjectContext>> (additionallyRequiredActions);
      _useParentContext = useParentContext;

      _prepareWaitForCompletionExecuted = false;
      _waitForCompletionExecuted = false;
    }

    public void PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (_prepareWaitForCompletionExecuted)
        throw new InvalidOperationException ("CompletionDetector cannot be reused, please create a new instance.");

      _prepareWaitForCompletionExecuted = true;

      OutputDebugMessage ("Started.");
      OutputDebugMessage ("Collecting state for completion detection.");

      var actualContext = DetermineContextForCompletionDetectionStrategies (context);
      foreach (var completionDetectionStrategy in _completionDetectionStrategies)
      {
        var state = completionDetectionStrategy.PrepareWaitForCompletion (actualContext);
        _completionDetectionStrategyStates.Add (completionDetectionStrategy, state);
      }
    }

    public void WaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (!_prepareWaitForCompletionExecuted)
        throw new InvalidOperationException ("PrepareWaitForCompletion must be called before WaitForCompletion.");

      if (_waitForCompletionExecuted)
        throw new InvalidOperationException ("CompletionDetector cannot be reused, please create a new instance.");

      _waitForCompletionExecuted = true;

      OutputDebugMessage ("Waiting for completion...");

      ExecuteAdditionallyRequiredActions (context);

      var actualContext = DetermineContextForCompletionDetectionStrategies (context);
      foreach (var completionDetectionStrategy in _completionDetectionStrategies)
      {
        var state = _completionDetectionStrategyStates[completionDetectionStrategy];
        completionDetectionStrategy.WaitForCompletion (actualContext, state);
      }

      OutputDebugMessage ("Finished.");
    }

    public void OutputDebugMessage (string message)
    {
      s_log.DebugFormat ("Action {0}: {1}", _debugOutputID, message);
    }

    private void ExecuteAdditionallyRequiredActions (PageObjectContext context)
    {
      foreach (var action in _additionallyRequiredActions)
        action (this, context);
    }

    private PageObjectContext DetermineContextForCompletionDetectionStrategies (PageObjectContext context)
    {
      if (_useParentContext)
      {
        OutputDebugMessage ("Using parent context for completion detection.");
        return context.ParentContext;
      }

      return context;
    }
  }
}