using System;
using JetBrains.Annotations;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="WebButton"/>.
  /// </summary>
  public class WebButtonControlObject : RemotionControlObject, IClickableControlObject
  {
    public WebButtonControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (IActionBehavior actionBehavior = null)
    {
      var actualClickBehavior = GetActualActionBehavior (actionBehavior);
      Scope.ClickAndWait (Context, actualClickBehavior);
      return UnspecifiedPage();
    }
  }
}