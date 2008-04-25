using System;
using System.Data;
using System.Globalization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class ValueConverter : ValueConverterBase
  {
    private readonly RdbmsProvider _provider;

    public ValueConverter (RdbmsProvider provider, TypeConversionProvider typeConversionProvider)
        : base (typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      _provider = provider;
    }

    public virtual object GetDBValue (object value)
    {
      if (value == null)
        return DBNull.Value;

      Type type = value.GetType();
      if (type.IsEnum)
        return Convert.ChangeType (value, Enum.GetUnderlyingType (type), CultureInfo.InvariantCulture);

      return value;
    }

    public virtual object GetDBValue (ObjectID id, string storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      if (id.StorageProviderID == storageProviderID)
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

      if (propertyDefinition.PropertyType != typeof (ObjectID))
        return GetValue (classDefinition, propertyDefinition, GetValue (dataReader, propertyDefinition.StorageSpecificName));
      else
        return GetObjectID (classDefinition, propertyDefinition, dataReader, propertyDefinition.StorageSpecificName);
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

      object idValue = GetValue (dataReader, "ID");
      if (idValue == DBNull.Value)
        return null;

      ClassDefinition classDefinition = GetClassDefinition (dataReader, idValue);
      return GetObjectID (classDefinition, idValue);
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

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[classID];
      if (classDefinition == null)
        throw _provider.CreateRdbmsProviderException ("Invalid ClassID '{0}' for ID '{1}' encountered.", classID, idValue);

      if (classDefinition.IsAbstract)
      {
        throw _provider.CreateRdbmsProviderException (
            "Invalid database value encountered. Column 'ClassID' of row with ID '{0}' refers to abstract class '{1}'.",
            idValue,
            classDefinition.ID);
      }

      return classDefinition;
    }

    private string GetClassID (IDataReader dataReader)
    {
      int classIDColumnOrdinal = GetMandatoryOrdinal (dataReader, "ClassID");
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

      OppositeClassDefinitionRetriever retriever = new OppositeClassDefinitionRetriever (_provider, classDefinition, propertyDefinition);
      ClassDefinition relatedClassDefinition = retriever.GetMandatoryOppositeClassDefinition (dataReader, objectIDColumnOrdinal);
      return GetObjectID (relatedClassDefinition, dataReader.GetValue (objectIDColumnOrdinal));
    }

    private void CheckObjectIDColumn (
        ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int objectIDColumnOrdinal)
    {
      IRelationEndPointDefinition endPointDefinition = classDefinition.GetMandatoryRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (endPointDefinition.IsMandatory && dataReader.IsDBNull (objectIDColumnOrdinal))
      {
        throw CreateConverterException (
            "Invalid null value for not-nullable relation property '{0}' encountered. Class: '{1}'.",
            propertyDefinition.PropertyName,
            classDefinition.ID);
      }
    }
  }
}