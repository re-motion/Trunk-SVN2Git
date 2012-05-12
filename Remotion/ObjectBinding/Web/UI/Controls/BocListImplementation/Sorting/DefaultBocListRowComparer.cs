// This file is part of the re-motion Core Framework (www.re-motion.org)
// CopyrowB (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  public class DefaultBocListRowComparer : IComparer<BocListRow>
  {
    private readonly Tuple<SortingDirection, IComparer<BocListRow>>[] _sorting;

    public DefaultBocListRowComparer (BocListSortingOrderEntry[] sortingOrder)
    {
      ArgumentUtility.CheckNotNull ("sortingOrder", sortingOrder);

      _sorting = sortingOrder
          .Where (entry => entry.Direction != SortingDirection.None)
          .Select (entry => Tuple.Create (entry.Direction, entry.Column.CreateCellValueComparer())).ToArray();
    }

    /// <summary>
    ///   Compares the current instance with another object of type <see cref="BocListRow"/>.
    /// </summary>
    /// <returns>
    ///   <list type="table">
    ///     <listheader>
    ///       <term> Value </term>
    ///       <description> Condition </description>
    ///     </listheader>
    ///     <item>
    ///       <term> Less than zero </term>
    ///       <description> <paramref name="rowA"/> is less than <paramref name="rowB"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> Zero </term>
    ///       <description> <paramref name="rowA"/> is equal to <paramref name="rowB"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> Greater than zero </term>
    ///       <description> <paramref name="rowA"/> is greater than <paramref name="rowB"/>. </description>
    ///     </item>
    ///   </list>
    /// </returns>
    public int Compare (BocListRow rowA, BocListRow rowB)
    {
      ArgumentUtility.CheckNotNull ("rowA", rowA);
      ArgumentUtility.CheckNotNull ("rowB", rowB);

      if (rowA == rowB)
        return 0;

      foreach (var sortingEntry in _sorting)
      {
        int compareResult = sortingEntry.Item2.Compare (rowA, rowB);
        if (compareResult != 0)
        {
          if (sortingEntry.Item1 == SortingDirection.Descending)
            return compareResult * -1;
          else
            return compareResult;
        }
      }

      return rowA.Index - rowB.Index;
    }
  }
}