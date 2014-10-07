using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="CommandControlObject"/>.
  /// </summary>
  public class CommandSelector : RemotionControlSelectorBase<CommandControlObject>
  {
    public CommandSelector ()
        : base ("a", "command")
    {
    }
  }
}