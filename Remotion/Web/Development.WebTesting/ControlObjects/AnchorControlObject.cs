using System;
using System.Web.UI.WebControls;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="LinkButton"/>, <see cref="HyperLink"/> and all their derivatives (e.g.
  /// <see cref="T:Remotion.Web.UI.Controls.WebLinkButton"/> or <see cref="T:Remotion.Web.UI.Controls.SmartHyperLink"/>). Also represents a simple
  /// HTML anchor &lt;a&gt; control within a re-motion applicaiton.
  /// </summary>
  [UsedImplicitly]
  public class AnchorControlObject : WebFormsControlObject, IClickableControlObject
  {
    public AnchorControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetector);
      return UnspecifiedPage();
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      if (IsPostBackLink(scope))
        return Continue.When (Wxe.PostBackCompleted);

      return Continue.When (Wxe.Reset);
    }

    private bool IsPostBackLink (ElementScope scope)
    {
      const string remotionDoPostBackScript = "DoPostBackWithOptions";
      const string aspNetDoPostBackScript = "__doPostBack";

      return scope["href"].Contains (remotionDoPostBackScript) ||
             scope["href"].Contains (aspNetDoPostBackScript) ||
             (scope["href"].Equals ("#") && scope["onclick"] != null && scope["onclick"].Contains (aspNetDoPostBackScript));
    }
  }
}