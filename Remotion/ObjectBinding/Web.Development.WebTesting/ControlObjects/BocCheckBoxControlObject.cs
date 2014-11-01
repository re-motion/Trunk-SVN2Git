using System;
using JetBrains.Annotations;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocCheckBoxControlObject : BocControlObject
  {
    public BocCheckBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the current state of the check box.
    /// </summary>
    public bool GetState ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return ParseState (FindChild ("Value")["data-value"]);

      return FindChild ("Value")["checked"] != null;
    }

    /// <summary>
    /// Sets the state of the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox"/> to <paramref name="newState"/>.
    /// </summary>
    public UnspecifiedPageObject SetTo (bool newState, ICompletionDetection completionDetection = null)
    {
      if (GetState() == newState)
        return UnspecifiedPage();

      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      FindChild ("Value").PerformAction (
          s =>
          {
            if (newState)
              s.Check();
            else
              s.Uncheck();
          },
          Context,
          actualCompletionDetector);

      return UnspecifiedPage();
    }

    private bool ParseState (string state)
    {
      if (state == "False")
        return false;

      if (state == "True")
        return true;

      throw new ArgumentException ("must be either 'True' or 'False'", "state");
    }
  }
}