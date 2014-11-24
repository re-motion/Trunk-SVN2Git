// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using log4net;
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
  public abstract class BocListControlObjectBase<TRowControlObject, TCellControlObject>
      : BocControlObject, IDropDownMenuHost, IListMenuHost, IControlObjectWithRows<TRowControlObject>, IFluentControlObjectWithRows<TRowControlObject>
      where TRowControlObject : ControlObject
      where TCellControlObject : ControlObject
  {
    /// <summary>
    /// Defines a list's column.
    /// </summary>
    protected class ColumnDefinition
    {
      private readonly string _itemID;
      private readonly int _index;
      private readonly string _title;
      private readonly bool _hasDiagnosticMetadata;

      public ColumnDefinition (string itemID, int index, string title, bool hasDiagnosticMetadata)
      {
        _itemID = itemID;
        _index = index;
        _title = title;
        _hasDiagnosticMetadata = hasDiagnosticMetadata;
      }

      public string ItemID
      {
        get { return _itemID; }
      }

      public int Index
      {
        get { return _index; }
      }

      public string Title
      {
        get { return _title; }
      }

      public bool HasDiagnosticMetadata
      {
        get { return _hasDiagnosticMetadata; }
      }
    }

    private class BocListRowControlObjectHostAccessor : IBocListRowControlObjectHostAccessor
    {
      private readonly BocListControlObjectBase<TRowControlObject, TCellControlObject> _bocList;

      public BocListRowControlObjectHostAccessor (BocListControlObjectBase<TRowControlObject, TCellControlObject> bocList)
      {
        _bocList = bocList;
      }

      public ElementScope ParentScope
      {
        get { return _bocList.Context.Scope; }
      }

      public int GetColumnIndex (string columnItemID)
      {
        return _bocList.GetColumnByItemID (columnItemID).Index;
      }
    }

    private readonly ILog _log;
    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly bool _hasFakeTableHead;
    private readonly List<ColumnDefinition> _columns;

    protected BocListControlObjectBase ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _log = LogManager.GetLogger (GetType());
      _accessor = new BocListRowControlObjectHostAccessor (this);

      EnsureBocListHasBeenFullyInitialized();

      _hasFakeTableHead = Scope.FindCss ("div.bocListTableContainer")[DiagnosticMetadataAttributesForObjectBinding.BocListHasFakeTableHead] != null;
      _columns = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (_hasFakeTableHead ? ".bocListFakeTableHead th" : ".bocListTableContainer th")
              .Select (
                  (s, i) =>
                      new ColumnDefinition (
                          s[DiagnosticMetadataAttributes.ItemID],
                          i + 1,
                          s[DiagnosticMetadataAttributes.Content],
                          ColumnHasDiagnosticMetadata (s)))
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
    /// Returns whether the list has a fake table head.
    /// </summary>
    protected bool HasFakeTableHead
    {
      get { return _hasFakeTableHead; }
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

    /// <summary>
    /// Returns the list's column titles. Columns without titles are represented as <see langword="null" /> strings.
    /// </summary>
    public IReadOnlyCollection<string> GetColumnTitles ()
    {
      return _columns.Select (cd => cd.Title).ToList();
    }

    /// <summary>
    /// Returns whether the list is empty.
    /// </summary>
    public bool IsEmpty ()
    {
      return GetRowCount() == 0;
    }

    /// <summary>
    /// Returns the list's empty message, call only if <see cref="IsEmpty"/> returns <see langword="true" />.
    /// </summary>
    /// <returns></returns>
    public string GetEmptyMessage ()
    {
      return Scope.FindCss (".bocListTable .bocListTableBody").Text.Trim();
    }

    /// <summary>
    /// Returns the number of rows in the list.
    /// </summary>
    public int GetRowCount ()
    {
      return RetryUntilTimeout.Run (() => Scope.FindAllCss (".bocListTable .bocListTableBody > tr.bocListDataRow").Count());
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithRows<TRowControlObject> GetRow ()
    {
      return this;
    }

    /// <inheritdoc/>
    public TRowControlObject GetRow (string itemID)
    {
      return GetRow().WithItemID (itemID);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRows<TRowControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributes.ItemID,
          itemID);
      return GetRowByCssSelector (cssSelector);
    }

    /// <inheritdoc/>
    TRowControlObject IFluentControlObjectWithRows<TRowControlObject>.WithIndex (int index)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex,
          index);
      return GetRowByCssSelector (cssSelector);
    }

    private TRowControlObject GetRowByCssSelector (string cssSelector)
    {
      var rowScope = Scope.FindCss (cssSelector);
      return CreateRowControlObject (GetHtmlID(), rowScope, _accessor);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }

    /// <inheritdoc/>
    public ListMenuControlObject GetListMenu ()
    {
      var listMenuScope = Scope.FindChild ("Boc_ListMenu");
      return new ListMenuControlObject (Context.CloneForControl (listMenuScope));
    }

    /// <summary>
    /// Returns the column defintion given by <paramref name="columnItemID"/>.
    /// </summary>
    protected ColumnDefinition GetColumnByItemID (string columnItemID)
    {
      return _columns.Single (cd => cd.ItemID == columnItemID);
    }

    /// <summary>
    /// Returns the column defintion given by <paramref name="index"/>.
    /// </summary>
    protected ColumnDefinition GetColumnByIndex (int index)
    {
      return _columns.Single (cd => cd.Index == index);
    }

    /// <summary>
    /// Returns the column defintion given by <paramref name="columnTitle"/>.
    /// </summary>
    protected ColumnDefinition GetColumnByTitle (string columnTitle)
    {
      return _columns.Single (cd => cd.Title == columnTitle);
    }

    private bool ColumnHasDiagnosticMetadata (ElementScope scope)
    {
      if (scope[DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasDiagnosticMetadata] == null)
        return false;

      return bool.Parse (scope[DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasDiagnosticMetadata]);
    }

    private void EnsureBocListHasBeenFullyInitialized ()
    {
      var bocListIsInitialized = RetryUntilTimeout.Run (() => Scope[DiagnosticMetadataAttributesForObjectBinding.BocListIsInitialized] == "true");
      if (!bocListIsInitialized)
        _log.WarnFormat ("Client side initialization of BocList '{0}' never finished.", GetHtmlID());
    }
  }
}