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
  [UsedImplicitly]
  public class BocReferenceValueControlObject : BocControlObject, ICommandHost, IDropDownMenuHost, ISelectableControlObject
  {
    /// <summary>
    /// Initializes the control object.
    /// </summary>
    /// <param name="id">The control object's ID.</param>
    /// <param name="context">The control object's context.</param>
    public BocReferenceValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return FindChild ("Label").Text; // do not trim

      return FindChild ("Value").GetSelectedOptionText();
    }

    public UnspecifiedPageObject SelectOption (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
      return SelectOption (selectAction, completionDetection);
    }

    public UnspecifiedPageObject SelectOption (int index, ICompletionDetection completionDetection = null)
    {
      Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
      return SelectOption (selectAction, completionDetection);
    }

    public UnspecifiedPageObject SelectOptionByText (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      
      Action<ElementScope> selectAction = s => s.SelectOption(text);
      return SelectOption (selectAction, completionDetection);
    }

    private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var actualCompletionDetection = DetermineActualCompletionDetection (completionDetection);
      FindChild ("Value").PerformAction (selectAction, Context, actualCompletionDetection);
      return UnspecifiedPage();
    }

    public CommandControlObject GetCommand ()
    {
      var commandScope = FindChild ("Command");
      var context = Context.CloneForScope (commandScope);
      return new CommandControlObject (commandScope.Id, context);
    }

    public UnspecifiedPageObject ExecuteCommand (ICompletionDetection completionDetection = null)
    {
      return GetCommand().Click (completionDetection);
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = FindChild ("Boc_OptionsMenu");
      var context = Context.CloneForScope (dropDownMenuScope);
      return new DropDownMenuControlObject (dropDownMenuScope.Id, context);
    }
  }
}