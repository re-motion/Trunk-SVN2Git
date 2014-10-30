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

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetection = DetermineActualCompletionDetection (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetection);
      return UnspecifiedPage();
    }
  }
}