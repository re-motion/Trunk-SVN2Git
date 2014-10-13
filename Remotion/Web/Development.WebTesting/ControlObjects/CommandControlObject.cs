using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a <see cref="T:Remotion.Web.UI.Controls.Command"/>.
  /// </summary>
  public class CommandControlObject : RemotionControlObject, IClickableControlObject
  {
    public CommandControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (IActionBehavior actionBehavior = null)
    {
      var actualWaitingStrategy = GetActualActionBehavior (actionBehavior);
      Scope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }
}