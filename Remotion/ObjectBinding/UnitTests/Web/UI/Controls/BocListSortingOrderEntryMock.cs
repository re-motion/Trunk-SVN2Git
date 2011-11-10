// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
