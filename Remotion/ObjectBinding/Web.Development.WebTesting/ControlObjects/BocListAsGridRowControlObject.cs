using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridRowControlObject : BocControlObject
  {
    // Todo RM-6297: Refactor code duplication with BocListRowControlObject and BocListEditableRowControlObject.

    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly int _rowIndex;

    public BocListAsGridRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _accessor = accessor;
      _rowIndex = int.Parse (Scope[DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex]);
    }

    public void ClickSelectCheckbox ()
    {
      var zeroBasedRowIndex = _rowIndex - 1;
      var rowSelectorCheckboxScope = FindChild (string.Format ("RowSelector_{0}", zeroBasedRowIndex));
      rowSelectorCheckboxScope.Click();
    }

    public BocListAsGridCellControlObject GetCell ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      var index = _accessor.GetColumnIndex (columnItemID);
      return GetCell (index);
    }

    public BocListAsGridCellControlObject GetCell (int index)
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index.ToString());
      return new BocListAsGridCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    [Obsolete ("BocList cells cannot be selected using a full HTML ID.", true)]
    public BocListCellControlObject GetCellByHtmlID ([NotNull] string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      throw new NotSupportedException ("BocList cells cannot be selected using a full HTML ID.");
    }

    public DropDownMenuControlObject GetRowDropDownMenu ()
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell, "true");
      var rowDropDownMenuScope = cellScope.FindCss ("span.DropDownMenuContainer");
      return new DropDownMenuControlObject (rowDropDownMenuScope.Id, Context.CloneForScope (rowDropDownMenuScope));
    }
  }
}