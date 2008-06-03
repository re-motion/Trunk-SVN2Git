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
