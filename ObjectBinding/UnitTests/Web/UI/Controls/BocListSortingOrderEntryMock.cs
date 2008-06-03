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

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

public class BocListSortingOrderEntryMock : BocListSortingOrderEntry
{
  public BocListSortingOrderEntryMock (IBocSortableColumnDefinition column, SortingDirection direction)
    : base (column, direction)
  {
  }

  public BocListSortingOrderEntryMock (int columnIndex, SortingDirection direction)
    : base (columnIndex, direction)
  {
  }

  public new int ColumnIndex
  {
    get { return base.ColumnIndex; }
  }

  public new void SetColumnIndex (int columnIndex)
  {
    base.SetColumnIndex (columnIndex);
  }

  public new void SetColumn (IBocSortableColumnDefinition column)
  {
    base.SetColumn (column); 
  }

  public new void SetDirection (SortingDirection direction)
  {
    base.SetDirection  (direction);
  }
}

}
