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
using System.Data;
using System.Globalization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Extends <see cref="ValueConverterBase"/> with functionality for reading database values from an <see cref="IDataReader"/> and converting
  /// them to .NET values, and for converting .NET values to database values.
  /// </summary>
  // TODO Review 4058: Extract interface, use in DB command builders, mock in DB command builder tests
  public class ValueConverter : ValueConverterBase
  {
    private readonly RdbmsProvider _provider;
    private readonly IStorageNameProvider _storageNameProvider;

    // TODO Review 4058: Remove provider parameter, use provider definition instead
    public ValueConverter (RdbmsProvider provider, IStorageNameProvider storageNameProvider, TypeConversionProvider typeConversionProvider)
        : base (typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);

      _provider = provider;
      _storageNameProvider = storageNameProvider;
    }

    public virtual object GetDBValue (object value)
    {
      if (value == null)
        return DBNull.Value;

      // TODO Review 4058: Check if ObjectID, if so, call GetDBValue overload for ObjectID

      Type type = value.GetType();
      if (type.IsEnum)
        return Convert.ChangeType (value, Enum.GetUnderlyingType (type), CultureInfo.InvariantCulture);

      var valueAsExtensibleEnum = value as IExtensibleEnum;
      if (valueAsExtensibleEnum != null)
        return valueAsExtensibleEnum.ID;

      return value;
    }

    // TODO Review 4058: Remove storageProviderID parameter, make private
    public virtual object GetDBValue (ObjectID id, string storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      // TODO Review 4058: Call IsOfSameStorageProvider here, moved from DbCommandBuilder
      if (id.StorageProviderDefinition.Name == storageProviderID)
        return id.Value;
      else
        return id.ToString();
    }

    public int GetMandatoryOrdinal (IDataReader dataReader, string columnName)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

      try
      {
        return dataReader.GetOrdinal (columnName);
      }
      catch (IndexOutOfRangeException)
      {
        throw _provider.CreateRdbmsProviderException ("The mandatory column '{0}' could not be found.", columnName);
      }
    }

    public object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (!propertyDefinition.IsObjectID)
        return GetValue (classDefinition, propertyDefinition, GetValue (dataReader, propertyDefinition.StoragePropertyDefinition.Name));
      else
        return GetObjectID (classDefinition, propertyDefinition, dataReader, propertyDefinition.StoragePropertyDefinition.Name);
    }

    public override object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (dataValue == DBNull.Value)
        dataValue = null;

      return base.GetValue (classDefinition, propertyDefinition, dataValue);
    }

    public ObjectID GetID (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      object idValue = GetValue (dataReader, _storageNameProvider.IDColumnName);
      if (idValue == DBNull.Value)
        return null;

      ClassDefinition classDefinition = GetClassDefinition (dataReader, idValue);
      return GetObjectID (classDefinition, idValue);
    }

    public object GetTimestamp (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      object timestamp = GetValue (dataReader, _storageNameProvider.TimestampColumnName);
      if (timestamp == DBNull.Value)
        return null;

      return timestamp;
    }

    public override ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (dataValue == DBNull.Value)
        dataValue = null;

      return base.GetObjectID (classDefinition, dataValue);
    }

    private object GetValue (IDataReader dataReader, string columnName)
    {
      return dataReader.GetValue (GetMandatoryOrdinal (dataReader, columnName));
    }

    private ClassDefinition GetClassDefinition (IDataReader dataReader, object idValue)
    {
      string classID = GetClassID (dataReader);

      var classDefinition = MappingConfiguration.Current.GetClassDefinition (
          classID,
          delegate { return _provider.CreateRdbmsProviderException ("Invalid ClassID '{0}' for ID '{1}' encountered.", classID, idValue); });

      if (classDefinition.IsAbstract)
      {
        throw _provider.CreateRdbmsProviderException (
            "Invalid database value encountered. Column '{0}' of row with ID '{1}' refers to abstract class '{2}'.",
            _storageNameProvider.ClassIDColumnName,
            idValue,
            classDefinition.ID);
      }

      return classDefinition;
    }

    private string GetClassID (IDataReader dataReader)
    {
      int classIDColumnOrdinal = GetMandatoryOrdinal (dataReader, _storageNameProvider.ClassIDColumnName);
      if (dataReader.IsDBNull (classIDColumnOrdinal))
        throw _provider.CreateRdbmsProviderException ("Invalid database value encountered. Column 'ClassID' must not contain null.");

      return dataReader.GetString (classIDColumnOrdinal);
    }

    private ObjectID GetObjectID (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, string columnName)
    {
      return GetObjectID (classDefinition, propertyDefinition, dataReader, GetMandatoryOrdinal (dataReader, columnName));
    }

    private ObjectID GetObjectID (
        ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int objectIDColumnOrdinal)
    {
      CheckObjectIDColumn (classDefinition, propertyDefinition, dataReader, objectIDColumnOrdinal);

      var retriever = new OppositeClassDefinitionRetriever (_provider, classDefinition, propertyDefinition, GetStorageNameProvider());
      ClassDefinition relatedClassDefinition = retriever.GetMandatoryOppositeClassDefinition (dataReader, objectIDColumnOrdinal);
      return GetObjectID (relatedClassDefinition, dataReader.GetValue (objectIDColumnOrdinal));
    }

    // TODO Review 4058: Use _storageNameProvider instead, remove method
    private IStorageNameProvider GetStorageNameProvider ()
    {
      return _provider.StorageProviderDefinition.Factory.CreateStorageNameProvider();
    }

    private void CheckObjectIDColumn (
        ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int objectIDColumnOrdinal)
    {
      if (dataReader.IsDBNull (objectIDColumnOrdinal))
      {
        var endPointDefinition = classDefinition.GetMandatoryRelationEndPointDefinition (propertyDefinition.PropertyName);
        if (endPointDefinition.IsMandatory)
        {
          throw CreateConverterException (
              "Invalid null value for not-nullable relation property '{0}' encountered. Class: '{1}'.",
              propertyDefinition.PropertyName,
              classDefinition.ID);
        }
      }
    }
  }
}
