using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocListAsGridControlObject : BocListControlObjectBase<BocListAsGridRowControlObject, BocListAsGridCellControlObject>
  {
    public BocListAsGridControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    protected override BocListAsGridRowControlObject CreateRowControlObject (
        string id,
        ElementScope rowScope,
        IBocListRowControlObjectHostAccessor accessor)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("rowScope", rowScope);
      ArgumentUtility.CheckNotNull ("accessor", accessor);

      return new BocListAsGridRowControlObject (accessor, Context.CloneForControl (rowScope));
    }

    protected override BocListAsGridCellControlObject CreateCellControlObject (string id, ElementScope cellScope)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("cellScope", cellScope);

      return new BocListAsGridCellControlObject (Context.CloneForControl (cellScope));
    }
  }
}