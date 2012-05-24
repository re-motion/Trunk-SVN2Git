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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  using Sorting = Tuple<SortingDirection, IComparer<BocListRow>>;

  public static class BocListRowSortExtension
  {
    public static IEnumerable<BocListRow> OrderBy (this IEnumerable<BocListRow> rows, BocListSortingOrderEntry[] sortingOrder)
    {
      ArgumentUtility.CheckNotNull ("rows", rows);
      ArgumentUtility.CheckNotNull ("sortingOrder", sortingOrder);

      var sorting = sortingOrder.GetSorting();

      if (!sorting.Any())
        return rows;

      return sorting.Skip (1).Aggregate (
          rows.OrderBy (sorting.First()),
          (current, entry) => current.ThenBy (entry))
          .ThenBy (r => r.Index);
    }

    private static Sorting[] GetSorting (this IEnumerable<BocListSortingOrderEntry> sortingOrder)
    {
      return sortingOrder
          .Where (entry => entry.Direction != SortingDirection.None)
          .Select (entry => Tuple.Create (entry.Direction, entry.Column.CreateCellValueComparer())).ToArray();
    }

    private static IOrderedEnumerable<BocListRow> OrderBy (this IEnumerable<BocListRow> rows, Sorting sorting)
    {
      if (sorting.Item1 == SortingDirection.Ascending)
        return rows.OrderBy (r => r, sorting.Item2);
      else
        return rows.OrderByDescending (r => r, sorting.Item2);
    }

    private static IOrderedEnumerable<BocListRow> ThenBy (this IOrderedEnumerable<BocListRow> rows2, Sorting sorting)
    {
      if (sorting.Item1 == SortingDirection.Ascending)
        return rows2.ThenBy (r => r, sorting.Item2);
      else
        return rows2.ThenByDescending (r => r, sorting.Item2);
    }
  }
}