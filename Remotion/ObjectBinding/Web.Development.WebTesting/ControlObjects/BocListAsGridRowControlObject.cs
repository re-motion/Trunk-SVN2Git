﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
  /// Control object representing a row within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridRowControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IDropDownMenuHost,
          IControlObjectWithCells<BocListAsGridCellControlObject>,
          IFluentControlObjectWithCells<BocListAsGridCellControlObject>
  {
    private readonly BocListRowFunctionality _impl;

    public BocListAsGridRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _impl = new BocListRowFunctionality (accessor, context);
    }

    /// <summary>
    /// Clicks the row's select checkbox (either selecting or deselecting the row).
    /// </summary>
    public void ClickSelectCheckbox ()
    {
      _impl.ClickSelectCheckbox();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithCells<BocListAsGridCellControlObject> GetCell ()
    {
      return this;
    }

    /// <inheritdoc/>
    public BocListAsGridCellControlObject GetCell (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return GetCell().WithColumnItemID (columnItemID);
    }

    /// <inheritdoc/>
    public BocListAsGridCellControlObject GetCell (int index)
    {
      return GetCell().WithIndex (index);
    }

    /// <inheritdoc/>
    BocListAsGridCellControlObject IFluentControlObjectWithCells<BocListAsGridCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListAsGridCellControlObject> (columnItemID);
    }

    /// <inheritdoc/>
    BocListAsGridCellControlObject IFluentControlObjectWithCells<BocListAsGridCellControlObject>.WithIndex (int index)
    {
      return _impl.GetCell<BocListAsGridCellControlObject> (index);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      return _impl.GetDropDownMenu();
    }
  }
}