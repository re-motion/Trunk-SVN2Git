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
  public class BocListAsGridRowControlObject : BocControlObject, IDropDownMenuHost
  {
    private readonly BocListRowFunctionality _impl;

    public BocListAsGridRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _impl = new BocListRowFunctionality (accessor, id, context);
    }

    public void ClickSelectCheckbox ()
    {
      _impl.ClickSelectCheckbox();
    }

    public BocListAsGridCellControlObject GetCell ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListAsGridCellControlObject> (columnItemID);
    }

    public BocListAsGridCellControlObject GetCell (int index)
    {
      return _impl.GetCell<BocListAsGridCellControlObject> (index);
    }

    [Obsolete ("BocList cells cannot be selected using a full HTML ID.", true)]
    public BocListCellControlObject GetCellByHtmlID ([NotNull] string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      throw new NotSupportedException ("BocList cells cannot be selected using a full HTML ID.");
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      return _impl.GetDropDownMenu();
    }
  }
}