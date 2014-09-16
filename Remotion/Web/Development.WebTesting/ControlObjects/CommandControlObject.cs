using System;
using JetBrains.Annotations;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  // Todo RM-6297: Integration tests? HowTo? Only in combination with "real" controls I guess?!

  /// <summary>
  /// Control object for <see cref="Command"/>.
  /// </summary>
  public class CommandControlObject : RemotionControlObject
  {
    public CommandControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (IWaitingStrategy waitStrategy = null)
    {
      var actualWaitStrategy = GetActualWaitingStrategy (waitStrategy);
      Scope.ClickAndWait (Context, actualWaitStrategy);
      return UnspecifiedPage();
    }
  }
}