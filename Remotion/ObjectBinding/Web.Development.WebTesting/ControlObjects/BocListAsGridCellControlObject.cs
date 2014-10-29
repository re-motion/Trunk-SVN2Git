using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridCellControlObject : BocControlObject, IControlHost
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

    /// <summary>
    /// Performs the cell's command.
    /// </summary>
    public UnspecifiedPageObject PerformCommand (IActionBehavior actionBehavior = null)
    {
      var actualWaitingStrategy = GetActualActionBehavior (actionBehavior);

      var commandScope = Scope.FindLink();
      commandScope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      return controlSelectionCommand.Select (Context);
    }
  }
}