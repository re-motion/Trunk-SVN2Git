using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class ConstantEnumerationValueFilter : IEnumerationValueFilter
  {
    private readonly Enum[] _disabledEnumValues;

    public ConstantEnumerationValueFilter (Enum[] disabledValues)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("disabledValues", disabledValues);
      ArgumentUtility.CheckItemsType ("disabledValues", disabledValues, disabledValues[0].GetType());

      _disabledEnumValues = disabledValues;
    }

    public Enum[] DisabledEnumValues
    {
      get { return _disabledEnumValues; }
    }

    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
    {
      return !Array.Exists (_disabledEnumValues, delegate (Enum disabledValue) { return disabledValue.Equals (value.Value); });
    }
  }
}