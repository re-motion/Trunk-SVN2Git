using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Common functionality of all control objects representing rows within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>. Specific
  /// classes (<see cref="BocListRowControlObject"/>, <see cref="BocListEditableRowControlObject"/> and
  /// <see cref="BocListAsGridRowControlObject"/>) serve only as different interfaces.
  /// </summary>
  internal class BocListRowFunctionality : BocControlObject
  {
    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly int _rowIndex;

    public BocListRowFunctionality (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _accessor = accessor;
      _rowIndex = int.Parse (Scope[DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex]);
    }

    public void ClickSelectCheckbox ()
    {
      var zeroBasedRowIndex = _rowIndex - 1;
      var rowSelectorCheckboxScope = _accessor.ParentScope.FindChild (string.Format ("RowSelector_{0}", zeroBasedRowIndex));
      rowSelectorCheckboxScope.Click();
    }

    public TCellControlObject GetCell<TCellControlObject> ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      var index = _accessor.GetColumnIndex (columnItemID);
      return GetCell<TCellControlObject> (index);
    }

    public TCellControlObject GetCell<TCellControlObject> (int index)
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index.ToString());
      return (TCellControlObject) Activator.CreateInstance (typeof (TCellControlObject), new object[] { Context.CloneForControl (cellScope) });
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell, "true");
      var rowDropDownMenuScope = cellScope.FindCss ("span.DropDownMenuContainer");
      return new DropDownMenuControlObject (Context.CloneForControl (rowDropDownMenuScope));
    }

    public BocListEditableRowControlObject Edit ()
    {
      var editCommandScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
      var editCommandLinkScope = editCommandScope.FindLink();
      editCommandLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());

      return new BocListEditableRowControlObject (_accessor, Context);
    }

    public BocListRowControlObject Save ()
    {
      var editCell = GetWellKnownEditCell();

      var save = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 1));
      save.Click();

      return new BocListRowControlObject (_accessor, Context);
    }

    public BocListRowControlObject Cancel ()
    {
      var editCell = GetWellKnownEditCell();

      var cancel = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 2));
      cancel.Click();

      return new BocListRowControlObject (_accessor, Context);
    }

    private BocListEditableCellControlObject GetWellKnownEditCell ()
    {
      var editCellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
      return new BocListEditableCellControlObject (Context.CloneForControl (editCellScope));
    }
  }
}