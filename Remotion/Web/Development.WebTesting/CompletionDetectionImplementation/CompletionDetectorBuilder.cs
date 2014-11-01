using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Implements the builder pattern for <see cref="ICompletionDetector"/> by implementing the fluent interface for <see cref="ICompletionDetection"/>
  /// and <see cref="IAdvancedCompletionDetection"/>.
  /// </summary>
  internal class CompletionDetectorBuilder : ICompletionDetection, IAdvancedCompletionDetection
  {
    private readonly List<ICompletionDetectionStrategy> _completionDetectionStrategies;
    private readonly List<Action<CompletionDetector, WebTestObjectContext>> _additionallyRequiredActions;
    private bool _useParentContext;

    public CompletionDetectorBuilder ()
    {
      _completionDetectionStrategies = new List<ICompletionDetectionStrategy>();
      _additionallyRequiredActions = new List<Action<CompletionDetector, WebTestObjectContext>>();
      _useParentContext = false;
    }

    public ICompletionDetector Build ()
    {
      return new CompletionDetector (_completionDetectionStrategies, _additionallyRequiredActions, _useParentContext);
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
          (cd, ctx) =>
          {
            cd.OutputDebugMessage ("Accepting modal browser dialog.");
            ctx.Browser.AcceptModalDialogFixed();
          });

      return this;
    }

    public IAdvancedCompletionDetection AndModalDialogHasBeenCanceled ()
    {
      _additionallyRequiredActions.Add (
          (cd, ctx) =>
          {
            cd.OutputDebugMessage ("Canceling modal browser dialog.");
            ctx.Browser.CancelModalDialogFixed();
          });

      return this;
    }
  }
}