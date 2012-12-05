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
using System.Collections;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Logging;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  public abstract class BocColumnDefinitionCellValueComparerBase: IComparer<BocListRow>
  {
    protected static readonly ILog s_log = LogManager.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private readonly ICache<Tuple<BocListRow, IBusinessObjectPropertyPath>, object> _rowValueCache =
        new Cache<Tuple<BocListRow, IBusinessObjectPropertyPath>, object>();

    public abstract int Compare (BocListRow rowA, BocListRow rowB);

    protected int? ComparePropertyPathValues (
        IBusinessObjectPropertyPath propertyPathA,
        BocListRow rowA,
        IBusinessObjectPropertyPath propertyPathB,
        BocListRow rowB)
    {
      object valueA = GetPropertyPathValueFromCache (rowA, propertyPathA);
      object valueB = GetPropertyPathValueFromCache (rowB, propertyPathB);

      if (valueA == null && valueB == null)
        return 0;
      if (valueA == null)
        return -1;
      if (valueB == null)
        return 1;

      IList listA = valueA as IList;
      IList listB = valueB as IList;
      if (listA != null || listB != null)
      {
        if (listA == null || listB == null)
          return 0;
        if (listA.Count == 0 && listB.Count == 0)
          return 0;
        if (listA.Count == 0)
          return -1;
        if (listB.Count == 0)
          return 1;
        valueA = listA[0];
        valueB = listB[0];
      }

      if (valueA is IComparable && valueB is IComparable)
        return Comparer.Default.Compare (valueA, valueB);
      else
        return null;
    }

    private object GetPropertyPathValueFromCache (BocListRow row, IBusinessObjectPropertyPath propertyPath)
    {
      if (propertyPath == null)
        return null;

      return _rowValueCache.GetOrCreateValue (CreateCacheKey (row, propertyPath), key => GetPropertyPathValue (key.Item1, key.Item2));
    }

    private object GetPropertyPathValue (BocListRow row, IBusinessObjectPropertyPath propertyPath)
    {
      try
      {
        var result = propertyPath.GetResult (
            row.BusinessObject,
            BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
            BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

        return result.GetValue();
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (
            e, "Exception thrown while reading string value for property path '{0}' in row {1} of BocList.", propertyPath, row.Index);
        return null;
      }
    }

    protected int CompareStrings (string valueA, string valueB)
    {
      return string.Compare (valueA, valueB);
    }

    private Tuple<BocListRow, IBusinessObjectPropertyPath> CreateCacheKey (BocListRow row, IBusinessObjectPropertyPath propertyPath)
    {
      return Tuple.Create (row, propertyPath);
    }
  }
}