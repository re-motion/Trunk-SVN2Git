using System;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="TextBox"/> and all its derivatives (none in re-motion).
  /// </summary>
  [UsedImplicitly]
  public class TextBoxControlObject : ControlObject, IFillableControlObject
  {
    public TextBoxControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      return Scope.Value; // do not trim
    }

    public UnspecifiedPageObject FillWith (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, completionDetection);
    }

    /// <remarks>
    /// The default <see cref="ICompletionDetection"/> for <see cref="TextBoxControlObject"/> does expect a WXE auto postback!
    /// </remarks>
    public UnspecifiedPageObject FillWith (string text, ThenAction then, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      var actualCompletionDetection = DetermineActualCompletionDetection (then, completionDetection);
      Scope.FillWithAndWait (text, then, Context, actualCompletionDetection);
      return UnspecifiedPage();
    }

    private ICompletionDetection DetermineActualCompletionDetection (ThenAction then, ICompletionDetection userDefinedCompletionDetection)
    {
      if (userDefinedCompletionDetection != null)
        return userDefinedCompletionDetection;

      return Behavior.WaitFor (then != Then.DoNothing ? WaitFor.WxePostBack : WaitFor.Nothing);
    }
  }
}