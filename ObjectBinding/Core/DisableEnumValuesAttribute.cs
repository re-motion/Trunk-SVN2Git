// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
