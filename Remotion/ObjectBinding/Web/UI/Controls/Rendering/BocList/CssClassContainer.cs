// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList
{
  /// <summary>
  /// Contains all the CSS class definitions needed throughout <see cref="BocList"/> rendering.
  /// </summary>
  public class CssClassContainer
  {
    private static readonly DoubleCheckedLockingContainer<CssClassContainer> s_instance =
        new DoubleCheckedLockingContainer<CssClassContainer> (() => new CssClassContainer());

    public static CssClassContainer Instance
    {
      get { return s_instance.Value; }
      set { s_instance.Value = value; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>table</c> tag. </summary>
    /// <remarks> Class: <c>bocListTable</c> </remarks>
    public virtual string Table
    {
      get { return "bocListTable"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>thead</c> tag. </summary>
    /// <remarks> Class: <c>bocListTableHead</c> </remarks>
    public virtual string TableHead
    {
      get { return "bocListTableHead"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>tbody</c> tag. </summary>
    /// <remarks> Class: <c>bocListTableBody</c> </remarks>
    public virtual string TableBody
    {
      get { return "bocListTableBody"; }
    }

    /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s title row. </summary>
    /// <remarks> Class: <c>bocListTitleCell</c> </remarks>
    public virtual string TitleCell
    {
      get { return "bocListTitleCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s selected data. </summary>
    /// <remarks> Class: <c>bocListDataRow</c> </remarks>
    public virtual string DataRow
    {
      get { return "bocListDataRow"; }
    }

    /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s selected data rows. </summary>
    /// <remarks> Class: <c>bocListDataRowSelected</c> </remarks>
    public virtual string DataRowSelected
    {
      get { return "bocListDataRowSelected"; }
    }

    /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s odd data rows. </summary>
    /// <remarks> Class: <c>bocListDataCellOdd</c> </remarks>
    public virtual string DataCellOdd
    {
      get { return "bocListDataCellOdd"; }
    }

    /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s even data rows. </summary>
    /// <remarks> Class: <c>bocListDataCellEven</c> </remarks>
    public virtual string DataCellEven
    {
      get { return "bocListDataCellEven"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s title row that contains the row index header. </summary>
    /// <remarks> Class: <c>bocListTitleCellIndex</c> </remarks>
    public virtual string TitleCellIndex
    {
      get { return "bocListTitleCellIndex"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data rows that contains the row index. </summary>
    /// <remarks> Class: <c>bocListDataCellIndex</c> </remarks>
    public virtual string DataCellIndex
    {
      get { return "bocListDataCellIndex"; }
    }

    /// <summary> Gets the CSS-Class applied to the content if there is no anchor element. </summary>
    /// <remarks> Class: <c>bocListDataCellContent</c> </remarks>
    public virtual string Content
    {
      get { return "bocListContent"; }
    }

    /// <summary> Gets the CSS-Class applied to the text providing the sorting order's index. </summary>
    /// <remarks> Class: <c>bocListSortingOrder</c> </remarks>
    public virtual string SortingOrder
    {
      get { return "bocListSortingOrder"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s navigator. </summary>
    /// <remarks> Class: <c>bocListNavigator</c> </remarks>
    public virtual string Navigator
    {
      get { return "bocListNavigator"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s list of additional columns. </summary>
    /// <remarks> Class: <c>bocListAvailableViewsListDropDownList</c> </remarks>
    public virtual string AvailableViewsListDropDownList
    {
      get { return "bocListAvailableViewsListDropDownList"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s label for the list of additional columns. </summary>
    /// <remarks> Class: <c>bocListAvailableViewsListLabel</c> </remarks>
    public virtual string AvailableViewsListLabel
    {
      get { return "bocListAvailableViewsListLabel"; }
    }

    public string MenuBlock
    {
      get { return "bocListMenuBlock"; }
    }

    public virtual string CommandText
    {
      get { return "bocListCommandText"; }
    }
  }
}