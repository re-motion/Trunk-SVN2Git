using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject : BocControlObject
  {
    private readonly BocListControlObject _bocList;

    public BocListEditableRowControlObject (BocListControlObject bocList, [NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _bocList = bocList;
    }

    public BocListRowControlObject Save ()
    {
      var editCell = GetWellKnownEditCell();

      var save = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 1));
      save.Click();

      return new BocListRowControlObject (_bocList, ID, Context);
    }

    public BocListRowControlObject Cancel ()
    {
      var editCell = GetWellKnownEditCell();

      var cancel = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 2));
      cancel.Click();

      return new BocListRowControlObject (_bocList, ID, Context);
    }

    public BocListEditableCellControlObject GetCell (string columnItemID)
    {
      var index = _bocList.GetColumnIndex (columnItemID);
      return GetCell (index);
    }

    public BocListEditableCellControlObject GetCell (int index)
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index.ToString());
      return new BocListEditableCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public BocListEditableCellControlObject GetCellByHtmlID (string htmlID)
    {
      throw new NotSupportedException ("BocList cells cannot be selected using a full HTML ID.");
    }

    private BocListEditableCellControlObject GetWellKnownEditCell ()
    {
      var editCellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
      return new BocListEditableCellControlObject (ID, Context.CloneForScope (editCellScope));
    }
  }
}