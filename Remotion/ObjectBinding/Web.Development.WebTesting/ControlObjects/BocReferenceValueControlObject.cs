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
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValue"/>.
  /// </summary>
  public class BocReferenceValueControlObject
      : BocControlObject,
          ICommandHost,
          IDropDownMenuHost,
          IControlObjectWithSelectableOptions,
          IFluentControlObjectWithSelectableOptions,
          IControlObjectWithText
  {
    public BocReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return Scope.FindChild ("Label").Text; // do not trim

      return Scope.FindChild ("Value").GetSelectedOptionText();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectOption (
        string itemID,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectOption().WithItemID (itemID, completionDetection);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (
        string itemID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
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
      Scope.FindChild ("Value").PerformAction (selectAction, Context, actualCompletionDetector, modalDialogHandler);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindChild ("Command");
      return new CommandControlObject (Context.CloneForControl (commandScope));
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject ExecuteCommand (ICompletionDetection completionDetection = null, IModalDialogHandler modalDialogHandler = null)
    {
      return GetCommand().Click (completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }
  }
}