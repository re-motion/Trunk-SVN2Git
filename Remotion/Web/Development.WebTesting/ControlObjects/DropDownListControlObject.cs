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
using System.Web.UI.WebControls;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="DropDownList"/>.
  /// </summary>
  public class DropDownListControlObject
      : WebFormsControlObject, IControlObjectWithSelectableOptions, IFluentControlObjectWithSelectableOptions, IControlObjectWithText
  {
    public DropDownListControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope.GetSelectedOptionText();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectOption (
        string value,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      return SelectOption().WithItemID (value, completionDetection);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (
        string value,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      Action<ElementScope> selectAction = s => s.SelectOptionByValue (value);
      return SelectOption (selectAction, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithIndex (
        int index,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
      return SelectOption (selectAction, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithDisplayText (
        string displayText,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNull ("displayText", displayText);

      Action<ElementScope> selectAction = s => s.SelectOption (displayText);
      return SelectOption (selectAction, completionDetection, modalDialogHandler);
    }

    private UnspecifiedPageObject SelectOption (
        [NotNull] Action<ElementScope> selectAction,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.PerformAction (selectAction, Context, actualCompletionDetector, modalDialogHandler);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      return Continue.When (Wxe.PostBackCompleted);
    }
  }
}