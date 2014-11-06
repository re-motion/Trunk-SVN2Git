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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Zero or more completion detection strategies are utilized each time a <see cref="ICompletionDetector"/> is run. A completion detection strategy
  /// blocks until the DOM fulfills certain characteristics (i.e. a page sequence counter has been increased). Note that completion detection
  /// strategies are re-used and therefore must be stateless.
  /// </summary>
  public interface ICompletionDetectionStrategy
  {
    /// <summary>
    /// Called immediately before the action is performed. The strategy should capture all state (e.g. a page sequence number) required for
    /// subsequently determining whether the action has completed.
    /// </summary>
    /// <param name="context">
    /// The <see cref="WebTestObjectContext"/> of the <see cref="ControlObject"/> with which the interaction takes place. Warning: the executed
    /// <see cref="ICompletionDetector"/> may decide to pass a different <see cref="WebTestObjectContext"/> to this method (e.g. the context of the
    /// parent window if the action closes the current window).
    /// </param>
    /// <returns>A state object which is subsequently passed to <see cref="WaitForCompletion"/>. May be null.</returns>
    object PrepareWaitForCompletion ([NotNull] PageObjectContext context);

    /// <summary>
    /// Called immediately after the action has been performed. This method should block until the DOM fulfills certain characteristics (i.e. a page
    /// sequence number has been increased). The object returned by <see cref="PrepareWaitForCompletion"/> is passed as <paramref name="state"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="WebTestObjectContext"/> of the <see cref="ControlObject"/> with which the interaction takes place. Warning: the executed
    /// <see cref="ICompletionDetector"/> may decide to pass a different <see cref="WebTestObjectContext"/> to this method (e.g. the context of the
    /// parent window if the action closes the current window).
    /// </param>
    /// <param name="state">The state object obtained from <see cref="PrepareWaitForCompletion"/>.</param>
    void WaitForCompletion ([NotNull] PageObjectContext context, [CanBeNull] object state);
  }
}