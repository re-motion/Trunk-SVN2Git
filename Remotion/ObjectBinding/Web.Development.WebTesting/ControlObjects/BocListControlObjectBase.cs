using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Common functionality of <see cref="BocListControlObject"/> and <see cref="BocListAsGridControlObject"/>.
  /// </summary>
  public abstract class BocListControlObjectBase<TRowControlObject, TCellControlObject> : BocControlObject, IDropDownMenuHost, IListMenuHost
      where TRowControlObject : ControlObject
      where TCellControlObject : ControlObject
  {
    private class ColumnDefinition
    {
      private readonly string _itemID;
      private readonly string _title;

      public ColumnDefinition (string itemID, string title)
      {
        _itemID = itemID;
        _title = title;
      }

      public string ItemID
      {
        get { return _itemID; }
      }

      public string Title
      {
        get { return _title; }
      }
    }

    private class BocListRowControlObjectHostAccessor : IBocListRowControlObjectHostAccessor
    {
      private readonly BocListControlObjectBase<TRowControlObject, TCellControlObject> _bocList;

      public BocListRowControlObjectHostAccessor (BocListControlObjectBase<TRowControlObject, TCellControlObject> bocList)
      {
        _bocList = bocList;
      }

      public string ID
      {
        get { return _bocList.GetHtmlID(); }
      }

      public int GetColumnIndex (string columnItemID)
      {
        return _bocList.GetColumnIndex (columnItemID);
      }
    }

    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly List<ColumnDefinition> _columns;

    protected BocListControlObjectBase ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _accessor = new BocListRowControlObjectHostAccessor (this);
      _columns = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (".bocListFakeTableHead th")
              .Select (s => new ColumnDefinition (s[DiagnosticMetadataAttributes.ItemID], s[DiagnosticMetadataAttributes.Text]))
              .ToList());
    }

    /// <summary>
    /// Returns a <see cref="IBocListRowControlObjectHostAccessor"/> for accessing the
    /// <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> from a row.
    /// </summary>
    protected IBocListRowControlObjectHostAccessor Accessor
    {
      get { return _accessor; }
    }

    /// <summary>
    /// Factory method: implementations instantiate their default row representation control object.
    /// </summary>
    protected abstract TRowControlObject CreateRowControlObject (
        [NotNull] string id,
        [NotNull] ElementScope rowScope,
        [NotNull] IBocListRowControlObjectHostAccessor accessor);

    /// <summary>
    /// Factory method: implementations instantiate their default cell representation control object.
    /// </summary>
    protected abstract TCellControlObject CreateCellControlObject ([NotNull] string id, [NotNull] ElementScope cellScope);

    public IReadOnlyCollection<string> GetColumnTitles ()
    {
      return _columns.Select (cd => cd.Title).ToList();
    }

    public int GetRowCount ()
    {
      return RetryUntilTimeout.Run (() => Scope.FindAllCss (".bocListTable .bocListTableBody tr").Count());
    }

    public TRowControlObject GetRow ([NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributes.ItemID,
          itemID);
      return GetRowByCssSelector (cssSelector);
    }

    public TRowControlObject GetRow (int index)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex,
          index);
      return GetRowByCssSelector (cssSelector);
    }

    [Obsolete ("BocList rows cannot be selected using a full HTML ID.", true)]
    public TRowControlObject GetRowByHtmlID ([NotNull] string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      // Method declaration exists for symmetry reasons only.

      throw new NotSupportedException ("BocList rows cannot be selected using a full HTML ID.");
    }

    private TRowControlObject GetRowByCssSelector (string cssSelector)
    {
      var rowScope = Scope.FindCss (cssSelector);
      return CreateRowControlObject (GetHtmlID(), rowScope, _accessor);
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }

    public ListMenuControlObject GetListMenu ()
    {
      var listMenuScope = FindChild ("Boc_ListMenu");
      return new ListMenuControlObject (Context.CloneForControl (listMenuScope));
    }

    protected int GetColumnIndex (string columnItemID)
    {
      var indexOf = _columns.IndexOf (cd => cd.ItemID == columnItemID);
      if (indexOf == -1)
        throw new KeyNotFoundException (string.Format ("Column item ID '{0}' does not exist.", columnItemID));

      return indexOf + 1;
    }

    protected int GetColumnIndexByTitle (string columnTitle)
    {
      var indexOf = _columns.IndexOf (cd => cd.Title == columnTitle);
      if (indexOf == -1)
        throw new KeyNotFoundException (string.Format ("Column title '{0}' does not exist.", columnTitle));

      return indexOf + 1;
    }
  }
}