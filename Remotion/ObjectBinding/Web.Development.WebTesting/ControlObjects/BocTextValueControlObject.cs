using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTextValue"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocTextValueControlObject : BocControlObject, IFillableControlObject
  {
    public BocTextValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
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

    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualCompletionDetector = GetActualCompletionDetector (finishInputWith, completionDetection);
      FindChild ("Value").FillWithAndWait (text, finishInputWith, Context, actualCompletionDetector);
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
  }
}