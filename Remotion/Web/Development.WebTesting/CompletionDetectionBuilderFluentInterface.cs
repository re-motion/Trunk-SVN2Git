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
using Remotion.Web.Development.WebTesting.CompletionDetectionImplementation;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Entry point of the fluent interface for the <see cref="ICompletionDetector"/> build.
  /// </summary>
  public static class Continue
  {
    /// <summary>
    /// Specifies a <see cref="ICompletionDetectionStrategy"/> after which executing of the web test should continue.
    /// </summary>
    public static IAdvancedCompletionDetection When ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy)
    {
      var completionDetector = new CompletionDetectorBuilder();
      completionDetector.And (completionDetectionStrategy);
      return completionDetector;
    }

    /// <summary>
    /// Specifies, that executing of the web test should continue immediately.
    /// </summary>
    public static IAdvancedCompletionDetection Immediately ()
    {
      return When (new NullCompletionDetectionStrategy());
    }

    // Todo RM-6297: Improve ControlObject.GetActualCompletionDetector() in order to enable Continue.Automatically().
    ///// <summary>
    ///// Specifies, that the action should automatically determine an appropriate completion detection strategy.
    ///// </summary>
    //public static IAdvancedCompletionDetection Automatically ()
    //{
    //  var completionDetector = new CompletionDetectorBuilder();
    //  return completionDetector;
    //}
  }

  /// <summary>
  /// Part of the fluent interface for building <see cref="ICompletionDetector"/> instances. Web test developers may specify various behavior which
  /// must be respected by the completion detector.
  /// </summary>
  public interface ICompletionDetection
  {
    /// <summary>
    /// Builds the <see cref="ICompletionDetector"/> instance.
    /// </summary>
    ICompletionDetector Build ();
  }

  /// <summary>
  /// Part of the fluent interface for building <see cref="ICompletionDetector"/> instances. This, "advanced", interface contains methods which should
  /// be available only after the initial <see cref="ICompletionDetectionStrategy"/> has been specified.
  /// </summary>
  public interface IAdvancedCompletionDetection : ICompletionDetection
  {
    /// <summary>
    /// Adds another <see cref="ICompletionDetectionStrategy"/> to the completion detector. The strategies are executed sequentially: first all
    /// <see cref="ICompletionDetectionStrategy.PrepareWaitForCompletion"/> calls, then the actual action, finally all
    /// <see cref="ICompletionDetectionStrategy.WaitForCompletion"/> calls (in the same order as before).
    /// </summary>
    /// <param name="completionDetectionStrategy">An additional <see cref="ICompletionDetectionStrategy"/>.</param>
    IAdvancedCompletionDetection And ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy);

    /// <summary>
    /// Informs the completion detector to wait for the current window to be closed. Note: this causes the completion detector to execute all
    /// <see cref="ICompletionDetectionStrategy"/>s on the parent window.
    /// </summary>
    IAdvancedCompletionDetection AndWindowHasClosed ();

    /// <summary>
    /// Accepts the modal browser dialog which is triggrered by the action.
    /// </summary>
    IAdvancedCompletionDetection AndModalDialogHasBeenAccepted ();

    /// <summary>
    /// Cancels the modal browser dialog which is triggered by the action.
    /// </summary>
    IAdvancedCompletionDetection AndModalDialogHasBeenCanceled ();
  }
}