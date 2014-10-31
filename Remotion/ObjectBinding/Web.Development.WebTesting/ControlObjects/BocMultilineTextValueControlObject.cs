
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
  [UsedImplicitly]
  public class BocMultilineTextValueControlObject : BocControlObject, IFillableControlObject
  {
    /// <summary>
    /// Initializes the control object.
    /// </summary>
    /// <param name="id">The control object's ID.</param>
    /// <param name="context">The control object's context.</param>
    public BocMultilineTextValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      var valueScope = FindChild ("Value");

      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return valueScope.Text; // do not trim

      return valueScope.Value; // do not trim
    }

    public UnspecifiedPageObject FillWith (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, FinishInput.WithTab, completionDetection);
    }

    public UnspecifiedPageObject FillWith ([NotNull] string[] lines, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("lines", lines);

      return FillWith (string.Join (Environment.NewLine, lines), completionDetection);
    }

    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualCompletionDetection = DetermineActualCompletionDetection (finishInputWith, completionDetection);
      FindChild ("Value").FillWithAndWait (text, finishInputWith, Context, actualCompletionDetection);
      return UnspecifiedPage();
    }

    public UnspecifiedPageObject FillWith ([NotNull] string[] lines, FinishInputWithAction finishInputWith, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("lines", lines);

      return FillWith (string.Join (Environment.NewLine, lines), finishInputWith, completionDetection);
    }

    private ICompletionDetection DetermineActualCompletionDetection (FinishInputWithAction finishInputWith, ICompletionDetection userDefinedCompletionDetection)
    {
      return finishInputWith == FinishInput.Promptly && userDefinedCompletionDetection == null
          ? Continue.Immediately()
          : DetermineActualCompletionDetection (userDefinedCompletionDetection);
    }
  }
}