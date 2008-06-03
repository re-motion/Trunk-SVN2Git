/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  public abstract class BaseBocListRowCompareValuesTest
  {
    protected void CompareEqualValuesAscending (IBocSortableColumnDefinition column, IBusinessObject left, IBusinessObject right)
    {
      BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
      sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Ascending);

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
      BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
      sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Descending);

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
      BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
      sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Ascending);

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
      BocListSortingOrderEntryMock[] sortingOrder = new BocListSortingOrderEntryMock[1];
      sortingOrder[0] = new BocListSortingOrderEntryMock (column, SortingDirection.Descending);

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
