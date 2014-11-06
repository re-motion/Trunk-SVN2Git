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
  /// Control object representing a row in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject : BocControlObject, IControlObjectWithCells<BocListEditableCellControlObject>
  {
    private readonly BocListRowFunctionality _impl;

    public BocListEditableRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _impl = new BocListRowFunctionality (accessor, context);
    }

    public BocListRowControlObject Save ()
    {
      return _impl.Save();
    }

    public BocListRowControlObject Cancel ()
    {
      return _impl.Cancel();
    }

    public IControlObjectWithCells<BocListEditableCellControlObject> GetCell ()
    {
      return this;
    }

    public BocListEditableCellControlObject GetCell (string columnItemID)
    {
      return GetCell().WithColumnItemID (columnItemID);
    }

    BocListEditableCellControlObject IControlObjectWithCells<BocListEditableCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      return _impl.GetCell<BocListEditableCellControlObject> (columnItemID);
    }

    BocListEditableCellControlObject IControlObjectWithCells<BocListEditableCellControlObject>.WithIndex (int index)
    {
      return _impl.GetCell<BocListEditableCellControlObject> (index);
    }
  }
}