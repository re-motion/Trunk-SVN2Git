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