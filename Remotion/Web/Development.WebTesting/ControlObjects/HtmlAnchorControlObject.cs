using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents a simple HTML anchor &lt;a&gt; control within a re-motion applicaiton.
  /// </summary>
  public class HtmlAnchorControlObject : HtmlControlObject, IClickableControlObject
  {
    public HtmlAnchorControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (IActionBehavior actionBehavior = null)
    {
      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      Scope.ClickAndWait (Context, actualActionBehavior);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Returns the actual <see cref="IActionBehavior"/> to be used.
    /// </summary>
    /// <param name="userDefinedActionBehavior">User-provided <see cref="IActionBehavior"/>.</param>
    /// <returns><see cref="IActionBehavior"/> to be used.</returns>
    private IActionBehavior GetActualActionBehavior ([CanBeNull] IActionBehavior userDefinedActionBehavior)
    {
      if (userDefinedActionBehavior != null)
        return userDefinedActionBehavior;

      if (IsPostBackLink())
        return Behavior.WaitFor (WaitFor.WxePostBack);

      return Behavior.WaitFor (WaitFor.WxeReset);
    }

    private bool IsPostBackLink ()
    {
      const string doPostBackScript = "__doPostBack";

      return Scope["href"].Contains (doPostBackScript)
             || (Scope["href"].Equals ("#") && Scope["onclick"] != null && Scope["onclick"].Contains (doPostBackScript));
    }
  }
}