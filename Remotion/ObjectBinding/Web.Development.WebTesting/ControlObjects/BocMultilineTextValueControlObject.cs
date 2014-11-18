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
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue"/>.
  /// </summary>
  public class BocMultilineTextValueControlObject : BocControlObject, IFillableControlObject
  {
    public BocMultilineTextValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      var valueScope = Scope.FindChild ("Value");

      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return valueScope.Text; // do not trim

      return valueScope.Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (
        string text,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Calls <see cref="FillWith(string,ICompletionDetection,IModalDialogHandler)"/> by joining the given lines with new line characters.
    /// </summary>
    public UnspecifiedPageObject FillWith (
        [NotNull] string[] lines,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("lines", lines);

      return FillWith (string.Join (Environment.NewLine, lines), completionDetection, modalDialogHandler);
    }

     /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (
        string text,
        FinishInputWithAction finishInputWith,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualCompletionDetector = GetActualCompletionDetector (finishInputWith, completionDetection);
      Scope.FindChild ("Value").FillInWithAndWait (text, finishInputWith, Context, actualCompletionDetector, modalDialogHandler);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Calls <see cref="FillWith(string,FinishInputWithAction,ICompletionDetection,IModalDialogHandler)"/> by joining the given lines with new line
    /// characters.
    /// </summary>
    public UnspecifiedPageObject FillWith (
        [NotNull] string[] lines,
        FinishInputWithAction finishInputWith,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("lines", lines);

      return FillWith (string.Join (Environment.NewLine, lines), finishInputWith, completionDetection, modalDialogHandler);
    }

    private ICompletionDetector GetActualCompletionDetector (
        FinishInputWithAction finishInputWith,
        ICompletionDetection userDefinedCompletionDetection)
    {
      if (finishInputWith == FinishInput.Promptly)
        return Continue.Immediately().Build();

      return GetActualCompletionDetector (userDefinedCompletionDetection);
    }
  }
}