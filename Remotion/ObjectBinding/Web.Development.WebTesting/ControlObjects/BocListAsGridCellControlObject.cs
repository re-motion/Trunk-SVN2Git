using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridCellControlObject : BocControlObject, IControlHost, ICommandHost
  {
    // Todo RM-6297: Refactor code duplication with BocListCellControlObject and BocListEditableCellControlObject.

    public BocListAsGridCellControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
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