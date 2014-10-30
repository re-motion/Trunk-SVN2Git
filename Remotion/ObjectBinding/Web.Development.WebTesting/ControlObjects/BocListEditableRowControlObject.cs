using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject : BocControlObject
  {
    private readonly BocListRowFunctionality _impl;

    public BocListEditableRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _impl = new BocListRowFunctionality (accessor, id, context);
    }

    public BocListRowControlObject Save ()
    {
      return _impl.Save();
    }

    public BocListRowControlObject Cancel ()
    {
      return _impl.Cancel();
    }

    public BocListEditableCellControlObject GetCell ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListEditableCellControlObject> (columnItemID);
    }

    public BocListEditableCellControlObject GetCell (int index)
    {
      return _impl.GetCell<BocListEditableCellControlObject> (index);
    }

    [Obsolete ("BocList cells cannot be selected using a full HTML ID.", true)]
    public BocListEditableCellControlObject GetCellByHtmlID ([NotNull] string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      throw new NotSupportedException ("BocList cells cannot be selected using a full HTML ID.");
    }
  }
}