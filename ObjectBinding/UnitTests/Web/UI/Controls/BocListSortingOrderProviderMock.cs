using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
public class BocListSortingOrderProviderMock : IBocListSortingOrderProvider
{
  BocListSortingOrderEntry[] _sortingOrder = new BocListSortingOrderEntryMock[0];

  public void SetSortingOrder (BocListSortingOrderEntry[] sortingOrder)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("sortingOrder", sortingOrder);
    _sortingOrder = sortingOrder;
  }

  public BocListSortingOrderEntry[] GetSortingOrder()
  {
    return _sortingOrder;
  }
}

}
