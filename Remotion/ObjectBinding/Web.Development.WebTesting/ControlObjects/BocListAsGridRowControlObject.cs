using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridRowControlObject : BocControlObject, IDropDownMenuHost, IControlObjectWithCells<BocListAsGridCellControlObject>
  {
    private readonly BocListRowFunctionality _impl;

    public BocListAsGridRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _impl = new BocListRowFunctionality (accessor, context);
    }

    public void ClickSelectCheckbox ()
    {
      _impl.ClickSelectCheckbox();
    }

    public IControlObjectWithCells<BocListAsGridCellControlObject> GetCell ()
    {
      return this;
    }

    public BocListAsGridCellControlObject GetCell (string columnItemID)
    {
      return GetCell().WithColumnItemID (columnItemID);
    }

    BocListAsGridCellControlObject IControlObjectWithCells<BocListAsGridCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListAsGridCellControlObject> (columnItemID);
    }

    BocListAsGridCellControlObject IControlObjectWithCells<BocListAsGridCellControlObject>.WithIndex (int index)
    {
      return _impl.GetCell<BocListAsGridCellControlObject> (index);
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      return _impl.GetDropDownMenu();
    }
  }
}