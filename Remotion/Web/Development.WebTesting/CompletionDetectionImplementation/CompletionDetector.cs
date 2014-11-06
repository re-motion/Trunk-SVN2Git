// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

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
      // Note: currently all framework-supported completion strategies already ensure this implicitly, keep for user-created completion strategies!
      actualContext.Window.EnsureWindowIsActive();
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