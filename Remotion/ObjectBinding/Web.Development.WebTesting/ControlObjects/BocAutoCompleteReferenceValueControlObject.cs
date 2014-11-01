using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocAutoCompleteReferenceValueControlObject : BocControlObject, IFillableControlObject, ICommandHost, IDropDownMenuHost
  {
    public BocAutoCompleteReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return Scope.FindChild ("Label").Text; // do not trim

      return Scope.FindChild ("TextValue").Value; // do not trim
    }

    public UnspecifiedPageObject FillWith (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, FinishInput.WithTab, completionDetection);
    }

    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.FindChild ("TextValue").FillWithAndWait (text, finishInputWith, Context, actualCompletionDetector);
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