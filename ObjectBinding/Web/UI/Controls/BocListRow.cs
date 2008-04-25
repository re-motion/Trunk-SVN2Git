using System;
using System.Collections;
using System.Collections.Specialized;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocListSortingOrderProvider
  {
    BocListSortingOrderEntry[] GetSortingOrder ();
  }

  public class BocListRow : IComparable
  {
    private static readonly ILog s_log = LogManager.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private IBocListSortingOrderProvider _provider;
    private int _index;
    private IBusinessObject _businessObject;
    private BocListSortingOrderEntry[] _cachedSortingOrder;

    private HybridDictionary _values;

    public BocListRow (IBocListSortingOrderProvider provider, int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      _provider = provider;
      _index = index;
      _businessObject = businessObject;
    }

    public int Index
    {
      get { return _index; }
      set { _index = value; }
    }

    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }

    private BocListSortingOrderEntry[] GetSortingOrder ()
    {
      if (_cachedSortingOrder == null)
        _cachedSortingOrder = _provider.GetSortingOrder();
      return _cachedSortingOrder;
    }

    private HybridDictionary Values
    {
      get
      {
        if (_values == null)
          _values = new HybridDictionary();
        return _values;
      }
    }

    int IComparable.CompareTo (object obj)
    {
      if (obj == null)
        return 1;
      BocListRow row = ArgumentUtility.CheckType<BocListRow> ("obj", obj);
      return CompareTo (row);
    }

    /// <summary>
    ///   Compares the current instance with another object of type <see cref="BocListRow"/>.
    /// </summary>
    /// <param name="row"> 
    ///   The <see cref="BocListRow"/> to compare with this instance.
    /// </param>
    /// <returns>
    ///   <list type="table">
    ///     <listheader>
    ///       <term> Value </term>
    ///       <description> Condition </description>
    ///     </listheader>
    ///     <item>
    ///       <term> Less than zero </term>
    ///       <description> This instance is less than <paramref name="row"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> Zero </term>
    ///       <description> This instance is equal to <paramref name="row"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> Greater than zero </term>
    ///       <description> This instance is greater than <paramref name="row"/>. </description>
    ///     </item>
    ///   </list>
    /// </returns>
    public int CompareTo (BocListRow row)
    {
      if (row == null)
        return 1;

      if (row == this)
        return 0;


      BocListSortingOrderEntry[] sortingOrder = GetSortingOrder();
      for (int i = 0; i < sortingOrder.Length; i++)
      {
        int compareResult = CompareToRowBySortingOrderEntry (row, sortingOrder[i]);
        if (compareResult != 0)
          return compareResult;
      }

      return Index - row.Index;
    }

    private int CompareToRowBySortingOrderEntry (BocListRow row, BocListSortingOrderEntry entry)
    {
      if (entry.Direction == SortingDirection.None)
        return 0;

      if (entry.Column is BocSimpleColumnDefinition)
      {
        return CompareToRowBySimpleColumn (row, entry);
      }
      else if (entry.Column is BocCustomColumnDefinition)
      {
        return CompareToRowByCustomColumn (row, entry);
      }
      else if (entry.Column is BocCompoundColumnDefinition)
      {
        return CompareToRowByCompundColumn (row, entry);
      }

      return 0;
    }

    private int CompareToRowBySimpleColumn (BocListRow row, BocListSortingOrderEntry entry)
    {
      BocSimpleColumnDefinition simpleColumn = (BocSimpleColumnDefinition) entry.Column;

      BusinessObjectPropertyPath propertyPathThis;
      BusinessObjectPropertyPath propertyPathRow;

      if (simpleColumn.IsDynamic)
      {
        // TODO: UnitTests
        propertyPathThis = simpleColumn.GetDynamicPropertyPath (BusinessObject.BusinessObjectClass);
        propertyPathRow = simpleColumn.GetDynamicPropertyPath (row.BusinessObject.BusinessObjectClass);
      }
      else
      {
        propertyPathThis = simpleColumn.GetPropertyPath();
        propertyPathRow = simpleColumn.GetPropertyPath();
      }

      int compareResult = 0;
      if (entry.Direction == SortingDirection.Ascending)
        compareResult = ComparePropertyPathValues (propertyPathThis, this, propertyPathRow, row);
      else if (entry.Direction == SortingDirection.Descending)
        compareResult = ComparePropertyPathValues (propertyPathRow, row, propertyPathThis, this);

      if (compareResult != 0)
        return compareResult;

      string stringValueA = GetStringValueForSimpleColumn (entry);
      string stringValueB = row.GetStringValueForSimpleColumn (entry);
      compareResult = CompareStrings (entry.Direction, stringValueA, stringValueB);

      return compareResult;
    }

    private int CompareToRowByCustomColumn (BocListRow row, BocListSortingOrderEntry entry)
    {
      BocCustomColumnDefinition customColumn = (BocCustomColumnDefinition) entry.Column;

      BusinessObjectPropertyPath propertyPathThis;
      BusinessObjectPropertyPath propertyPathRow;

      if (customColumn.IsDynamic)
      {
        // TODO: UnitTests
        propertyPathThis = customColumn.GetDynamicPropertyPath (BusinessObject.BusinessObjectClass);
        propertyPathRow = customColumn.GetDynamicPropertyPath (row.BusinessObject.BusinessObjectClass);
      }
      else
      {
        propertyPathThis = customColumn.GetPropertyPath();
        propertyPathRow = customColumn.GetPropertyPath();
      }

      int compareResult = 0;
      if (entry.Direction == SortingDirection.Ascending)
        compareResult = ComparePropertyPathValues (propertyPathThis, this, propertyPathRow, row);
      else if (entry.Direction == SortingDirection.Descending)
        compareResult = ComparePropertyPathValues (propertyPathRow, row, propertyPathThis, this);

      if (compareResult != 0)
        return compareResult;

      string stringValueA = GetStringValueForCustomColumn (entry);
      string stringValueB = row.GetStringValueForCustomColumn (entry);
      compareResult = CompareStrings (entry.Direction, stringValueA, stringValueB);

      return compareResult;
    }

    private int CompareToRowByCompundColumn (BocListRow row, BocListSortingOrderEntry entry)
    {
      BocCompoundColumnDefinition compoundColumn = (BocCompoundColumnDefinition) entry.Column;

      int compareResult = 0;
      for (int idxBindings = 0; idxBindings < compoundColumn.PropertyPathBindings.Count; idxBindings++)
      {
        PropertyPathBinding propertyPathBinding = compoundColumn.PropertyPathBindings[idxBindings];
        BusinessObjectPropertyPath propertyPathThis;
        BusinessObjectPropertyPath propertyPathRow;

        if (propertyPathBinding.IsDynamic)
        {
          // TODO: UnitTests
          propertyPathThis = propertyPathBinding.GetDynamicPropertyPath (BusinessObject.BusinessObjectClass);
          propertyPathRow = propertyPathBinding.GetDynamicPropertyPath (row.BusinessObject.BusinessObjectClass);
        }
        else
        {
          propertyPathThis = propertyPathBinding.GetPropertyPath();
          propertyPathRow = propertyPathBinding.GetPropertyPath();
        }

        if (entry.Direction == SortingDirection.Ascending)
          compareResult = ComparePropertyPathValues (propertyPathThis, this, propertyPathRow, row);
        else if (entry.Direction == SortingDirection.Descending)
          compareResult = ComparePropertyPathValues (propertyPathRow, row, propertyPathThis, this);

        if (compareResult != 0)
          return compareResult;
      }

      string stringValueA = GetStringValueForCompoundColumn (entry);
      string stringValueB = row.GetStringValueForCompoundColumn (entry);
      compareResult = CompareStrings (entry.Direction, stringValueA, stringValueB);

      return compareResult;
    }

    private int ComparePropertyPathValues (
        BusinessObjectPropertyPath propertyPathA,
        BocListRow rowA,
        BusinessObjectPropertyPath propertyPathB,
        BocListRow rowB)
    {
      object valueA = rowA.GetPropertyPathValue (propertyPathA);
      object valueB = rowB.GetPropertyPathValue (propertyPathB);

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

      return 0;
      //Better leave the comparisson of non-ICompareables to the calling method. ToString is not always the right choice.
      //return Comparer.Default.Compare (valueA.ToString(), valueB.ToString());
    }

    private object GetPropertyPathValue (BusinessObjectPropertyPath propertyPath)
    {
      if (propertyPath == null)
        return null;

      object value = Values[propertyPath];
      if (value == null)
      {
        try
        {
          value = propertyPath.GetValue (BusinessObject, false, true);
        }
        catch (Exception e)
        {
          s_log.ErrorFormat (e, "Exception thrown while reading string value for property path '{0}' in row {1} of BocList.", propertyPath, _index);
        }
        Values[propertyPath] = value;
      }
      return value;
    }

    private string GetStringValueForSimpleColumn (BocListSortingOrderEntry entry)
    {
      BocSimpleColumnDefinition column = (BocSimpleColumnDefinition) entry.Column;

      string value = (string) Values[column];
      if (value == null)
      {
        try
        {
          value = column.GetStringValue (BusinessObject);
        }
        catch (Exception e)
        {
          s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", column.ItemID, _index);
        }
        Values[column] = value;
      }
      return value;
    }

    private string GetStringValueForCustomColumn (BocListSortingOrderEntry entry)
    {
      BocCustomColumnDefinition column = (BocCustomColumnDefinition) entry.Column;

      string value = (string) Values[column];
      if (value == null)
      {
        try
        {
          //TODO: Support DynamicPropertyPaths
          BusinessObjectPropertyPath propertyPath = column.GetPropertyPath();
          value = propertyPath.GetString (BusinessObject, string.Empty);
        }
        catch (Exception e)
        {
          s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", column.ItemID, _index);
        }
        Values[column] = value;
      }
      return value;
    }

    private string GetStringValueForCompoundColumn (BocListSortingOrderEntry entry)
    {
      BocCompoundColumnDefinition column = (BocCompoundColumnDefinition) entry.Column;

      string value = (string) Values[column];
      if (value == null)
      {
        try
        {
          value = column.GetStringValue (BusinessObject);
        }
        catch (Exception e)
        {
          s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", column.ItemID, _index);
        }
        Values[column] = value;
      }
      return value;
    }

    private int CompareStrings (SortingDirection direction, string valueA, string valueB)
    {
      int compareResult = 0;

      if (direction == SortingDirection.Ascending)
        compareResult = string.Compare (valueA, valueB);
      else if (direction == SortingDirection.Descending)
        compareResult = string.Compare (valueB, valueA);

      return compareResult;
    }
  }
}