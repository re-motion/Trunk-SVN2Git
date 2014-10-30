using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Common functionality of all control objects representing cells within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>. Specific
  /// classes (<see cref="BocListCellControlObject"/>, <see cref="BocListEditableCellControlObject"/> and
  /// <see cref="BocListAsGridCellControlObject"/>) serve only as different interfaces.
  /// </summary>
  internal class BocListCellFunctionality : BocControlObject, ICommandHost, IControlHost
  {
    public BocListCellFunctionality ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the text content of the cell.
    /// </summary>
    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindLink();
      var context = Context.CloneForScope (commandScope);
      return new CommandControlObject (commandScope.Id, context);
    }

    public UnspecifiedPageObject ExecuteCommand (IActionBehavior actionBehavior = null)
    {
      return GetCommand().Click (actionBehavior);
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}