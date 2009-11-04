// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Globalization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  public class ValueConverterBase
  {
    private TypeConversionProvider _typeConversionProvider;

    protected ValueConverterBase (TypeConversionProvider typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull ("typeConversionServices", typeConversionProvider);

      _typeConversionProvider = typeConversionProvider;
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

    public virtual ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (dataValue == null)
        return null;

      if (dataValue.GetType() == typeof (string))
      {
        ObjectID id = null;
        try
        {
          id = ObjectID.Parse ((string) dataValue);
        }
        catch (ArgumentException)
        {
        }
        catch (FormatException)
        {
        }

        if (id != null)
          return id;
      }

      return new ObjectID (classDefinition.ID, dataValue);
    }

    public virtual object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (dataValue == null)
      {
        if (!propertyDefinition.IsNullable)
        {
          throw CreateConverterException (
              "Invalid null value for not-nullable property '{0}' encountered. Class: '{1}'.",
              propertyDefinition.PropertyName,
              classDefinition.ID);
        }

        return propertyDefinition.DefaultValue;        
      }

      return GetNativeObjectFromValue (classDefinition, propertyDefinition, dataValue);
    }

    protected virtual object GetNativeObjectFromValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("dataValue", dataValue);

      try
      {
        return TypeConversionProvider.Convert (null, CultureInfo.InvariantCulture, dataValue.GetType(), propertyDefinition.PropertyType, dataValue);
      }
      catch (Exception e)
      {
        throw CreateConverterException (
            e,
            "Error converting the value for property '{0}' of class '{1}' from persistence medium:\r\n{2}",
            propertyDefinition.PropertyName,
            classDefinition.ID,
            e.Message);
      }
    }

    protected ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), argumentName);
    }

    protected ConverterException CreateConverterException (string formatString, params object[] args)
    {
      return CreateConverterException (null, formatString, args);
    }

    protected ConverterException CreateConverterException (Exception innerException, string formatString, params object[] args)
    {
      return new ConverterException (string.Format (formatString, args), innerException);
    }
  }
}
