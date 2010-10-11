// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  public class BocColumnRendererArrayBuilder
  {
    private readonly BocColumnDefinition[] _columnDefinitions;
    private readonly IServiceLocator _serviceLocator;
    private readonly WcagHelper _wcagHelper;

    public BocColumnRendererArrayBuilder (BocColumnDefinition[] columnDefinitions, IServiceLocator serviceLocator, WcagHelper wcagHelper)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnDefinitions", columnDefinitions);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);
      ArgumentUtility.CheckNotNull ("wcagHelper", wcagHelper);

      _columnDefinitions = columnDefinitions;
      _serviceLocator = serviceLocator;
      _wcagHelper = wcagHelper;
    }

    public bool EnableIcon { get; set; }
    public bool IsListReadOnly { get; set; }
    public bool IsListEditModeActive { get; set; }
    public bool IsBrowserCapableOfScripting { get; set; }
    public bool IsClientSideSortingEnabled { get; set; }
    public bool HasSortingKeys { get; set; }
    public List<BocListSortingOrderEntry> SortingOrder { get; set; }
    
    public BocColumnRenderer[] CreateColumnRenderers ()
    {
      var sortingDirections = new Dictionary<int, SortingDirection> ();
      var sortingOrder = new List<int> ();
      
      PrepareSorting (sortingDirections, sortingOrder);
      //var entries = SortingOrder.Where (e => e.Direction != SortingDirection.None).Select ((e, i) => new { Entry = e, OrderIndex = i }).ToDictionary (result.Entry.ColumnINdex, result);

      var firstValueColumnRendered = false;
      var bocColumnRenderers = new List<BocColumnRenderer> (_columnDefinitions.Length);
      for (int columnIndex = 0; columnIndex < _columnDefinitions.Length; columnIndex++)
      {
        var columnDefinition = _columnDefinitions[columnIndex];
        var showIcon = !firstValueColumnRendered && columnDefinition is BocValueColumnDefinition && EnableIcon;

        var sortingDirection = SortingDirection.None;
        if (sortingDirections.ContainsKey (columnIndex))
          sortingDirection = sortingDirections[columnIndex];
        var orderIndex = sortingOrder.IndexOf (columnIndex);

        if (IsColumnVisible (columnDefinition))
        {
          var columnRenderer = columnDefinition.GetRenderer (_serviceLocator);
          bocColumnRenderers.Add (new BocColumnRenderer (columnRenderer, columnDefinition, columnIndex, showIcon, sortingDirection, orderIndex));
        }
        else
          bocColumnRenderers.Add (new BocColumnRenderer (new NullColumnRenderer(), columnDefinition, columnIndex, false, sortingDirection, orderIndex));

        if (columnDefinition is BocValueColumnDefinition)
          firstValueColumnRendered = true;
      }
      return bocColumnRenderers.ToArray();
    }

    private void PrepareSorting (IDictionary<int, SortingDirection> sortingDirections, IList<int> sortingOrder)
    {
      if (IsClientSideSortingEnabled || HasSortingKeys)
      {
        foreach (var entry in SortingOrder)
        {
          sortingDirections[entry.ColumnIndex] = entry.Direction;
          if (entry.Direction != SortingDirection.None)
            sortingOrder.Add (entry.ColumnIndex);
        }
      }
    }

    private bool IsColumnVisible (BocColumnDefinition column)
    {
      ArgumentUtility.CheckNotNull ("column", column);

      var columnAsCommandColumn = column as BocCommandColumnDefinition;
      if (columnAsCommandColumn != null && columnAsCommandColumn.Command != null)
      {
        if (!IsColumnVisibleForBocCommandColumnDefinition(columnAsCommandColumn))
          return false;
      }

      var columnAsRowEditModeColumn = column as BocRowEditModeColumnDefinition;
      if (columnAsRowEditModeColumn != null)
      {
        if (!IsColumnVisibleForBocRowEditModeColumnDefinition(columnAsRowEditModeColumn))
          return false;
      }

      var columnAsDropDownMenuColumn = column as BocDropDownMenuColumnDefinition;
      if (columnAsDropDownMenuColumn != null)
        return IsColumnVisibleForBocDropDownMenuColumnDefinition (columnAsDropDownMenuColumn);
      
      return true;
    }

    private bool IsColumnVisibleForBocCommandColumnDefinition (BocCommandColumnDefinition columnAsCommandColumn)
    {
      if (_wcagHelper.IsWaiConformanceLevelARequired ()
          && (columnAsCommandColumn.Command.Type == CommandType.Event || columnAsCommandColumn.Command.Type == CommandType.WxeFunction))
        return false;
      return true;
    }

    private bool IsColumnVisibleForBocRowEditModeColumnDefinition (BocRowEditModeColumnDefinition column)
    {
      if (_wcagHelper.IsWaiConformanceLevelARequired ())
        return false;
      if (column.Show == BocRowEditColumnDefinitionShow.EditMode && IsListReadOnly)
        return false;
      if (IsListEditModeActive)
        return false;
      return true;
    }

    private bool IsColumnVisibleForBocDropDownMenuColumnDefinition (BocDropDownMenuColumnDefinition column)
    {
      if (_wcagHelper.IsWaiConformanceLevelARequired ())
        return false;
      return IsBrowserCapableOfScripting;
    }
    
  }
}