using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="WebButton"/>.
  /// </summary>
  public class WebButtonControlObject : RemotionControlObject
  {
    public WebButtonControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base(id, context)
    {
    }

    public UnspecifiedPageObject Click (IWaitingStrategy waitStrategy = null)
    {
      // Todo RM-6297: todo todo todo here
      var actualWaitStrategy = GetActualWaitingStrategy (waitStrategy);
      Scope.ClickAndWait (Context, actualWaitStrategy);
      return UnspecifiedPage();
    }
  }
}