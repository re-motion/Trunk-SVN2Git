using System;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTextValue"/> enriched by a TinyMCE editor.
  /// </summary>
  public class ActaNovaTinyMceControlObject : BocControlObject, IFillableControlObject
  {
    public ActaNovaTinyMceControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the current markup contained in the TinyMCE editor control.
    /// </summary>
    public string GetMarkup ()
    {
      var tinyMceFrameBodyScope = GetTinyMceFrameBodyScope();
      return tinyMceFrameBodyScope.InnerHTML;
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      var tinyMceFrameBodyScope = GetTinyMceFrameBodyScope();
      return tinyMceFrameBodyScope.Text.Trim().Replace ("<br>", Environment.NewLine);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (
        string text,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (
        string text,
        FinishInputWithAction finishInputWith,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var translatedText = text.Replace (Environment.NewLine, "<br>").Replace ("\n", "<br>");
      return FillWithMarkup (translatedText, finishInputWith, completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Fills the control with the given <paramref name="markup"/>.
    /// </summary>
    /// <param name="markup">The markup to fill in.</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <param name="modalDialogHandler">Required <see cref="IModalDialogHandler"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    public UnspecifiedPageObject FillWithMarkup (
        string markup,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("markup", markup);

      return FillWithMarkup (markup, FinishInput.WithTab, completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Fills the control with the given <paramref name="markup"/> and afterwards executes the given <paramref name="finishInputWith"/> action.
    /// </summary>
    /// <param name="markup">The markup to fill in.</param>
    /// <param name="finishInputWith">What to do after the text has been filled in (see <see cref="FinishInput"/> for supported default options).</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <param name="modalDialogHandler">Required <see cref="IModalDialogHandler"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    public UnspecifiedPageObject FillWithMarkup (
        string markup,
        FinishInputWithAction finishInputWith,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("markup", markup);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var securedMarkup = "<p>" + markup.Replace ("'", "\'") + "</p>";
      var script = string.Format ("document.body.innerHTML='{0}';", securedMarkup);
      Context.Browser.Driver.ExecuteScript (script, GetTinyMceFrameBodyScope());

      var actualCompletionDetector = GetActualCompletionDetector (finishInputWith, completionDetection);
      GetTinyMceFrameBodyScope().PerformAction (s => s.SendKeysFixed(Keys.Tab), Context, actualCompletionDetector, modalDialogHandler);

      return UnspecifiedPage();
    }

    private ElementScope GetTinyMceFrameBodyScope ()
    {
      var tinyMceFrameName = GetHtmlID() + "_Value_ifr";
      var tinyMceFrame = Scope.FindFrame (tinyMceFrameName);
      var tinyMceFrameBodyScope = tinyMceFrame.FindCss ("body");
      return tinyMceFrameBodyScope;
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