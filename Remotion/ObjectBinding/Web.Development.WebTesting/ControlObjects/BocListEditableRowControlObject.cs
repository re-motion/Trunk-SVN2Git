using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject : BocControlObject, IControlObjectWithCells<BocListEditableCellControlObject>
  {
    private readonly BocListRowFunctionality _impl;

    public BocListEditableRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _impl = new BocListRowFunctionality (accessor, context);
    }

    public BocListRowControlObject Save ()
    {
      return _impl.Save();
    }

    public BocListRowControlObject Cancel ()
    {
      return _impl.Cancel();
    }

    public IControlObjectWithCells<BocListEditableCellControlObject> GetCell ()
    {
      return this;
    }

    public BocListEditableCellControlObject GetCell (string columnItemID)
    {
      return GetCell().WithColumnItemID (columnItemID);
    }

    BocListEditableCellControlObject IControlObjectWithCells<BocListEditableCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListEditableCellControlObject> (columnItemID);
    }

    BocListEditableCellControlObject IControlObjectWithCells<BocListEditableCellControlObject>.WithIndex (int index)
    {
      return _impl.GetCell<BocListEditableCellControlObject> (index);
    }
  }
}