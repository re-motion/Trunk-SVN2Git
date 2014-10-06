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

    public UnspecifiedPageObject Click (IWaitingStrategy waitingStrategy = null)
    {
      var actualWaitingStrategy = GetActualWaitingStrategy (waitingStrategy);
      Scope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Returns the waiting strategy to be used.
    /// </summary>
    /// <param name="waitingStrategy">User-provided waiting strategy.</param>
    /// <returns>Waiting strategy to be used.</returns>
    private IWaitingStrategy GetActualWaitingStrategy ([CanBeNull] IWaitingStrategy waitingStrategy)
    {
      if (waitingStrategy != null)
        return waitingStrategy;

      if (IsPostBackLink())
        return WaitFor.WxePostBack;

      return WaitFor.WxeReset;
    }

    private bool IsPostBackLink ()
    {
      return Scope["href"].Contains ("__doPostBack");
    }
  }
}