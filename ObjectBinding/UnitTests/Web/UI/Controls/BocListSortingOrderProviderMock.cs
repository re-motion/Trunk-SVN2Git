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
