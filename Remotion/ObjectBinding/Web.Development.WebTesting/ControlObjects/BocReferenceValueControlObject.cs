﻿using System;
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
  [UsedImplicitly]
  public class BocReferenceValueControlObject : BocControlObject, ICommandHost, IDropDownMenuHost, IControlObjectWithSelectableOptions
  {
    public BocReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return Scope.FindChild ("Label").Text; // do not trim

      return Scope.FindChild ("Value").GetSelectedOptionText();
    }

    public IControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    public UnspecifiedPageObject SelectOption (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectOption().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableOptions.WithItemID (string itemID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
      return SelectOption (selectAction, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableOptions.WithIndex (int index, ICompletionDetection completionDetection)
    {
      Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
      return SelectOption (selectAction, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableOptions.WithText (string text, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      Action<ElementScope> selectAction = s => s.SelectOption (text);
      return SelectOption (selectAction, completionDetection);
    }

    private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.FindChild ("Value").PerformAction (selectAction, Context, actualCompletionDetector);
      return UnspecifiedPage();
    }

    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindChild ("Command");
      var context = Context.CloneForControl (commandScope);
      return new CommandControlObject (context);
    }

    public UnspecifiedPageObject ExecuteCommand (ICompletionDetection completionDetection = null)
    {
      return GetCommand().Click (completionDetection);
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      var context = Context.CloneForControl (dropDownMenuScope);
      return new DropDownMenuControlObject (context);
    }
  }
}