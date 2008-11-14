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
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  //TODO: doc
  [AttributeUsage (AttributeTargets.Enum | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class DisableEnumValuesAttribute : Attribute
  {
    private IEnumerationValueFilter _filter;

    public DisableEnumValuesAttribute (Type filterType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("filterType", filterType, typeof (IEnumerationValueFilter));

      _filter = (IEnumerationValueFilter) TypesafeActivator.CreateInstance (filterType).With();
    }

    public DisableEnumValuesAttribute (params object[] disabledEnumValues)
    {
      ArgumentUtility.CheckNotNull ("disabledEnumValues", disabledEnumValues);
      ArgumentUtility.CheckItemsType ("disabledEnumValues", disabledEnumValues, typeof (Enum));

      Initialize ((Enum[]) ArrayUtility.Convert (disabledEnumValues, typeof (Enum)));
    }

    public DisableEnumValuesAttribute (object disabledEnumValue1)
    {
      Initialize (
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue1", disabledEnumValue1));
    }

    public DisableEnumValuesAttribute (object disabledEnumValue1, object disabledEnumValue2)
    {
      Initialize (
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue2", disabledEnumValue2));
    }

    public DisableEnumValuesAttribute (object disabledEnumValue1, object disabledEnumValue2, object disabledEnumValue3)
    {
      Initialize (
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue2", disabledEnumValue2),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue3", disabledEnumValue3));
    }

    public DisableEnumValuesAttribute (object disabledEnumValue1, object disabledEnumValue2, object disabledEnumValue3, object disabledEnumValue4)
    {
      Initialize (
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue2", disabledEnumValue2),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue3", disabledEnumValue3),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue4", disabledEnumValue4));
    }

    public DisableEnumValuesAttribute (
        object disabledEnumValue1, object disabledEnumValue2, object disabledEnumValue3, object disabledEnumValue4, object disabledEnumValue5)
    {
      Initialize (
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue2", disabledEnumValue2),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue3", disabledEnumValue3),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue4", disabledEnumValue4),
          ArgumentUtility.CheckNotNullAndType<Enum> ("disabledEnumValue5", disabledEnumValue5));
    }

    private void Initialize (params Enum[] disabledEnumValues)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("disabledEnumValues", disabledEnumValues);
      ArgumentUtility.CheckItemsType ("disabledEnumValues", disabledEnumValues, disabledEnumValues[0].GetType());

      _filter = new ConstantEnumerationValueFilter (disabledEnumValues);
    }

    public IEnumerationValueFilter GetEnumerationValueFilter ()
    {
      return _filter;
    }
  }
}
