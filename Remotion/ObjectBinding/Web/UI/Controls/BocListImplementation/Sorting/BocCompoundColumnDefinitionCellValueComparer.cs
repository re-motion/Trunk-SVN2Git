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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  public class BocCompoundColumnDefinitionCellValueComparer : BocColumnDefinitionCellValueComparerBase
  {
    private readonly ICache<BocListRow, string> _stringValueCache = new Cache<BocListRow, string>();
    private readonly BocCompoundColumnDefinition _column;

    public BocCompoundColumnDefinitionCellValueComparer (BocCompoundColumnDefinition column)
    {
      ArgumentUtility.CheckNotNull ("column", column);
      _column = column;
    }

    public override int Compare (BocListRow rowA, BocListRow rowB)
    {
      ArgumentUtility.CheckNotNull ("rowA", rowA);
      ArgumentUtility.CheckNotNull ("rowB", rowB);

      for (int idxBindings = 0; idxBindings < _column.PropertyPathBindings.Count; idxBindings++)
      {
        PropertyPathBinding propertyPathBinding = _column.PropertyPathBindings[idxBindings];

        var propertyPathRowA = propertyPathBinding.GetPropertyPath();
        var propertyPathRowB = propertyPathBinding.GetPropertyPath();

        var compareResult = ComparePropertyPathValues (propertyPathRowA, rowA, propertyPathRowB, rowB);

        if (!compareResult.HasValue)
          break;

        if (compareResult.Value != 0)
          return compareResult.Value;
      }

      string stringValueA = _stringValueCache.GetOrCreateValue (rowA, GetStringValueForCompoundColumn);
      string stringValueB = _stringValueCache.GetOrCreateValue (rowB, GetStringValueForCompoundColumn);
      return CompareStrings (stringValueA, stringValueB);
    }

    private string GetStringValueForCompoundColumn (BocListRow row)
    {
      try
      {
        return _column.GetStringValue (row.BusinessObject);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", _column.ItemID, row.Index);
        return null;
      }
    }
  }
}