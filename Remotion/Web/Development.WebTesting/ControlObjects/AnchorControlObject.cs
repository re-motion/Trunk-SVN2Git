using System;
using System.Web.UI.WebControls;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="LinkButton"/>, <see cref="HyperLink"/> and all their derivatives (e.g.
  /// <see cref="T:Remotion.Web.UI.Controls.WebLinkButton"/> or <see cref="T:Remotion.Web.UI.Controls.SmartHyperLink"/>). Also represents a simple
  /// HTML anchor &lt;a&gt; control within a re-motion applicaiton.
  /// </summary>
  [UsedImplicitly]
  public class AnchorControlObject : ControlObject, IClickableControlObject
  {
    public AnchorControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetection = DetermineActualCompletionDetection (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetection);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Returns the actual <see cref="ICompletionDetection"/> to be used.
    /// </summary>
    /// <param name="userDefinedCompletionDetection">User-provided <see cref="ICompletionDetection"/>.</param>
    /// <returns><see cref="ICompletionDetection"/> to be used.</returns>
    private ICompletionDetection DetermineActualCompletionDetection (ICompletionDetection userDefinedCompletionDetection)
    {
      if (userDefinedCompletionDetection != null)
        return userDefinedCompletionDetection;

      if (IsPostBackLink())
        return Continue.When (Wxe.PostBackCompleted);

      return Continue.When (Wxe.Reset);
    }

    private bool IsPostBackLink ()
    {
      const string remotionDoPostBackScript = "DoPostBackWithOptions";
      const string aspNetDoPostBackScript = "__doPostBack";

      return Scope["href"].Contains (remotionDoPostBackScript) ||
             Scope["href"].Contains (aspNetDoPostBackScript) ||
             (Scope["href"].Equals ("#") && Scope["onclick"] != null && Scope["onclick"].Contains (aspNetDoPostBackScript));
    }
  }
}