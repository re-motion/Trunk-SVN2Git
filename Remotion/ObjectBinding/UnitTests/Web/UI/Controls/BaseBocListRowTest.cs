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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  public abstract class BaseBocListRowCompareValuesTest
  {
    protected void CompareEqualValuesAscending (IBocSortableColumnDefinition column, IBusinessObject left, IBusinessObject right)
    {
      BocListSortingOrderEntry[] sortingOrder = new BocListSortingOrderEntry[1];
      sortingOrder[0] = new BocListSortingOrderEntry (column, SortingDirection.Ascending);

      BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
      provider.SetSortingOrder (sortingOrder);

      BocListRow rowLeft = new BocListRow (provider, 0, left);
      BocListRow rowRight = new BocListRow (provider, 0, right);

      int compareResultLeftRight = rowLeft.CompareTo (rowRight);
      int compareResultRightLeft = rowRight.CompareTo (rowLeft);

      Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
      Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
    }

    protected void CompareEqualValuesDescending (IBocSortableColumnDefinition column, IBusinessObject left, IBusinessObject right)
    {
      BocListSortingOrderEntry[] sortingOrder = new BocListSortingOrderEntry[1];
      sortingOrder[0] = new BocListSortingOrderEntry (column, SortingDirection.Descending);

      BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
      provider.SetSortingOrder (sortingOrder);

      BocListRow rowLeft = new BocListRow (provider, 0, left);
      BocListRow rowRight = new BocListRow (provider, 0, right);

      int compareResultLeftRight = rowLeft.CompareTo (rowRight);
      int compareResultRightLeft = rowRight.CompareTo (rowLeft);

      Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
      Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
    }

    protected void CompareAscendingValuesAscending (IBocSortableColumnDefinition column, IBusinessObject left, IBusinessObject right)
    {
      BocListSortingOrderEntry[] sortingOrder = new BocListSortingOrderEntry[1];
      sortingOrder[0] = new BocListSortingOrderEntry (column, SortingDirection.Ascending);

      BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
      provider.SetSortingOrder (sortingOrder);

      BocListRow rowLeft = new BocListRow (provider, 0, left);
      BocListRow rowRight = new BocListRow (provider, 0, right);

      int compareResultLeftRight = rowLeft.CompareTo (rowRight);
      int compareResultRightLeft = rowRight.CompareTo (rowLeft);

      Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
      Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
    }

    protected void CompareAscendingValuesDescending (IBocSortableColumnDefinition column, IBusinessObject left, IBusinessObject right)
    {
      BocListSortingOrderEntry[] sortingOrder = new BocListSortingOrderEntry[1];
      sortingOrder[0] = new BocListSortingOrderEntry (column, SortingDirection.Descending);

      BocListSortingOrderProviderMock provider = new BocListSortingOrderProviderMock();
      provider.SetSortingOrder (sortingOrder);

      BocListRow rowLeft = new BocListRow (provider, 0, left);
      BocListRow rowRight = new BocListRow (provider, 0, right);

      int compareResultLeftRight = rowLeft.CompareTo (rowRight);
      int compareResultRightLeft = rowRight.CompareTo (rowLeft);

      Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
      Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
    }
  }
}
