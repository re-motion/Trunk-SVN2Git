using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  // Todo RM-6297: Integration tests? HowTo? Only in combination with "real" controls I guess?!

  /// <summary>
  /// Control object representing a <see cref="Command"/>.
  /// </summary>
  public class CommandControlObject : RemotionControlObject, IClickable
  {
    public CommandControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (IWaitingStrategy waitingStrategy = null)
    {
      var actualWaitingStrategy = GetActualWaitingStrategy (waitingStrategy);
      Scope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }
}