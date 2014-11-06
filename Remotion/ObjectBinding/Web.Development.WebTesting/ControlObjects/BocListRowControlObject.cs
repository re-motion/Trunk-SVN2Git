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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListRowControlObject : BocControlObject, IDropDownMenuHost, IControlObjectWithCells<BocListCellControlObject>
  {
    private readonly BocListRowFunctionality _impl;

    public BocListRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _impl = new BocListRowFunctionality (accessor, context);
    }

    public void ClickSelectCheckbox ()
    {
      _impl.ClickSelectCheckbox();
    }

    public BocListEditableRowControlObject Edit ()
    {
      return _impl.Edit();
    }

    public IControlObjectWithCells<BocListCellControlObject> GetCell ()
    {
      return this;
    }

    public BocListCellControlObject GetCell (string columnItemID)
    {
      return GetCell().WithColumnItemID (columnItemID);
    }

    BocListCellControlObject IControlObjectWithCells<BocListCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListCellControlObject> (columnItemID);
    }

    BocListCellControlObject IControlObjectWithCells<BocListCellControlObject>.WithIndex (int index)
    {
      return _impl.GetCell<BocListCellControlObject> (index);
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      return _impl.GetDropDownMenu();
    }
  }
}