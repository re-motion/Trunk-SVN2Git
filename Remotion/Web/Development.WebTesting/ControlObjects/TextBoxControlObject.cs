using System;
using System.Web.UI.WebControls;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="TextBox"/> and all its derivatives (none in re-motion).
  /// </summary>
  [UsedImplicitly]
  public class TextBoxControlObject : WebFormsControlObject, IFillableControlObject
  {
    public TextBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      return Scope.Value; // do not trim
    }

    public UnspecifiedPageObject FillWith (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, FinishInput.WithTab, completionDetection);
    }

    /// <remarks>
    /// The default <see cref="ICompletionDetection"/> for <see cref="TextBoxControlObject"/> does expect a WXE auto postback!
    /// </remarks>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualCompletionDetector = GetActualCompletionDetector (finishInputWith, completionDetection);
      Scope.FillWithAndWait (text, finishInputWith, Context, actualCompletionDetector);
      return UnspecifiedPage();
    }

    private ICompletionDetector GetActualCompletionDetector (
        FinishInputWithAction finishInputWith,
        ICompletionDetection userDefinedCompletionDetection)
    {
      if (finishInputWith == FinishInput.Promptly)
        return Continue.Immediately().Build();

      return GetActualCompletionDetector (userDefinedCompletionDetection);
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return Continue.When (Wxe.PostBackCompleted);
    }
  }
}