using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing an arbitrary AppTools-based page.
  /// </summary>
  public class AppToolsPageObject : RemotionPageObject
  {
    // ReSharper disable once MemberCanBeProtected.Global
    public AppToolsPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Perform (string itemID, ICompletionDetection completionDetection = null)
    {
      var fullItemID = string.Format ("{0}Button", itemID);
      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), fullItemID));
      return webButton.Click (completionDetection);
    }

    public UnspecifiedPageObject ClickImage (string localID, ICompletionDetection completionDetection = null)
    {
      // Todo RM-6297: Provide a control object for abused input image tags?
      var actualCompletionDetection = completionDetection ?? Continue.When (Wxe.PostBackCompleted);

      var fullLocalID = string.Format ("{0}Button", localID);
      var image = Scope.FindIdEndingWith (fullLocalID);
      var imageContext = Context.CloneForControl(this, image);
      image.ClickAndWait (imageContext, actualCompletionDetection.Build());
      return new UnspecifiedPageObject (imageContext);
    }
  }
}