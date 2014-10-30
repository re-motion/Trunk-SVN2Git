using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocBooleanValueControlObject : BocControlObject
  {
    public BocBooleanValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the current state of the boolean.
    /// </summary>
    public bool? GetState ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return ParseState (FindChild ("Value")["data-value"]);

      return ParseState (FindChild ("Value").Value);
    }

    /// <summary>
    /// Returns whether this instance supports tri-state.
    /// </summary>
    public bool IsTriState ()
    {
      return Scope[DiagnosticMetadataAttributesForObjectBinding.BocBooleanValueIsTriState] == "true";
    }

    /// <summary>
    /// Sets the state of the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue"/> to <paramref name="newState"/>.
    /// </summary>
    public UnspecifiedPageObject SetTo (bool? newState, ICompletionDetection completionDetection = null)
    {
      if (!IsTriState() && !newState.HasValue)
        throw new ArgumentException ("Must not be null for non-tri-state BocBooleanValue controls.", "newState");

      if (GetState() == newState)
        return UnspecifiedPage();

      if (!IsTriState())
        return Click (1, completionDetection);

      var states = new bool?[] { false, null, true, false, null };
      var numberOfClicks = Array.LastIndexOf (states, newState) - Array.IndexOf (states, GetState());
      return Click (numberOfClicks, completionDetection);
    }

    private UnspecifiedPageObject Click (int numberOfClicks, ICompletionDetection completionDetection)
    {
      var actualCompletionDetection = DetermineActualCompletionDetection (completionDetection);

      var linkScope = FindChild ("DisplayValue");

      for (var i = 0; i < numberOfClicks; ++i)
        linkScope.ClickAndWait (Context, actualCompletionDetection);

      return UnspecifiedPage();
    }

    private bool? ParseState (string state)
    {
      if (state == "False")
        return false;

      if (state == "True")
        return true;

      return null;
    }
  }
}