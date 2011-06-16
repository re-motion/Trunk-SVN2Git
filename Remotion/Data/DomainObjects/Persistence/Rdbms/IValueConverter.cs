using System;
using System.Data;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public interface IValueConverter
  {
    bool IsOfSameStorageProvider (ObjectID id);
    object GetDBValue (object value);
    int GetMandatoryOrdinal (IDataReader dataReader, string columnName);
    object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader);
    object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue);
    ObjectID GetID (IDataReader dataReader);
    object GetTimestamp (IDataReader dataReader);
    ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue);
  }
}