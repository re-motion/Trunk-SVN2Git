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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="ElementScope"/> class, which - after the action has been performed on the DOM element - wait
  /// using a given <see cref="ICompletionDetection"/>.
  /// </summary>
  public static class CoypuWaitingElementScopeExtensions
  {
    /// <summary>
    /// Performs an <paramref name="action"/> on a DOM element (given by <paramref name="scope"/>), which is part of a control object (represented by
    /// its <paramref name="context"/>) using the given <paramref name="completionDetector"/>.
    /// </summary>
    /// <param name="scope">The DOM element.</param>
    /// <param name="action">Action to be performed on the DOM element.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="completionDetector"><see cref="ICompletionDetector"/> for this action.</param>
    public static void PerformAction (
        [NotNull] this ElementScope scope,
        [NotNull] Action<ElementScope> action,
        [NotNull] ControlObjectContext context,
        [NotNull] ICompletionDetector completionDetector)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("action", action);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("completionDetector", completionDetector);

      var pageContext = context.PageObject.Context;
      completionDetector.PrepareWaitForCompletion (pageContext);
      action (scope);
      completionDetector.WaitForCompletion (pageContext);
    }

    /// <summary>
    /// Performs a click on a DOM element (given by <paramref name="scope"/>), which is part of a control object (represented by its
    /// <paramref name="context"/>) using the given <paramref name="completionDetector"/>.
    /// </summary>
    /// <param name="scope">The DOM element.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="completionDetector"><see cref="ICompletionDetector"/> for this action.</param>
    public static void ClickAndWait (
        [NotNull] this ElementScope scope,
        [NotNull] ControlObjectContext context,
        [NotNull] ICompletionDetector completionDetector)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("completionDetector", completionDetector);

      scope.PerformAction (s => s.FocusClick(), context, completionDetector);
    }

    /// <summary>
    /// Fills the given DOM input element (given by <paramref name="scope"/>), which is part of a control object (represented by its
    /// <paramref name="context"/>) with the given <paramref name="value"/> using the given <paramref name="finishInputWithAction"/> and
    /// <paramref name="completionDetector"/>.
    /// </summary>
    /// <param name="scope">The DOM input element.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="finishInputWithAction"><see cref="FinishInputWithAction"/> for this action.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="completionDetector"><see cref="ICompletionDetector"/> for this action.</param>
    public static void FillInWithAndWait (
        [NotNull] this ElementScope scope,
        [NotNull] string value,
        [NotNull] FinishInputWithAction finishInputWithAction,
        [NotNull] ControlObjectContext context,
        [NotNull] ICompletionDetector completionDetector)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("finishInputWithAction", finishInputWithAction);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("completionDetector", completionDetector);

      scope.PerformAction (s => s.FillInWithFixed (value, finishInputWithAction), context, completionDetector);
    }
  }
}