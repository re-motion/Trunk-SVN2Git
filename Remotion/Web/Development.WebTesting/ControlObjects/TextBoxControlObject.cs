using System;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="TextBox"/> and all its derivatives (none in re-motion).
  /// </summary>
  [UsedImplicitly]
  public class TextBoxControlObject : ControlObject, IFillableControlObject
  {
    public TextBoxControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      return Scope.Value;
    }

    public UnspecifiedPageObject FillWith (string text, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, actionBehavior);
    }

    /// <remarks>
    /// The default <see cref="IActionBehavior"/> for <see cref="TextBoxControlObject"/> does expect a WXE auto postback!
    /// </remarks>
    public UnspecifiedPageObject FillWith (string text, ThenAction then, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      if (actionBehavior == null)
        actionBehavior = Behavior.WaitFor (WaitFor.WxePostBack);

      Scope.FillWithAndWait (text, then, Context, actionBehavior);
      return UnspecifiedPage();
    }
  }
}