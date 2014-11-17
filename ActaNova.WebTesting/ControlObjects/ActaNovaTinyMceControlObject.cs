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

    public string GetMarkup ()
    {
      var tinyMceFrameBodyScope = GetTinyMceFrameBodyScope();
      return tinyMceFrameBodyScope.InnerHTML;
    }

    public string GetText ()
    {
      var tinyMceFrameBodyScope = GetTinyMceFrameBodyScope();
      return tinyMceFrameBodyScope.Text.Trim().Replace ("<br>", Environment.NewLine);
    }

    public UnspecifiedPageObject FillWith (
        string text,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, completionDetection, modalDialogHandler);
    }

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

    public UnspecifiedPageObject FillWithMarkup (
        string markup,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("markup", markup);

      return FillWithMarkup (markup, FinishInput.WithTab, completionDetection, modalDialogHandler);
    }

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
      var tinyMceFrameName = Scope.Id + "_Value_ifr";
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