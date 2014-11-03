using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="CommandControlObject"/>.
  /// </summary>
  public class CommandSelector : TypedControlSelectorBase<CommandControlObject>
  {
    public CommandSelector ()
        : base ("Command")
    {
    }

    protected override CommandControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new CommandControlObject (newControlObjectContext);
    }
  }
}