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