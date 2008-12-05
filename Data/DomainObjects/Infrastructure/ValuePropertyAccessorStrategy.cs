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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  internal class ValuePropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly ValuePropertyAccessorStrategy Instance = new ValuePropertyAccessorStrategy();

    private ValuePropertyAccessorStrategy ()
    {
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      return propertyDefinition.PropertyType;
    }

    private PropertyValue GetPropertyValue (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return ((DataContainer) ((ClientTransaction) transaction).GetDataContainer (propertyAccessor.DomainObject)).PropertyValues[propertyAccessor.PropertyData.PropertyIdentifier];
    }

    public bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetPropertyValue (propertyAccessor, transaction).HasChanged;
    }

    public bool HasBeenTouched (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetPropertyValue (propertyAccessor, transaction).HasBeenTouched;
    }

    public bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetValueWithoutTypeCheck (propertyAccessor, transaction) == null;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetPropertyValue (propertyAccessor, transaction).Value;
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      GetPropertyValue (propertyAccessor, transaction).Value = value;
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return GetPropertyValue (propertyAccessor, transaction).OriginalValue;
    }
  }
}
