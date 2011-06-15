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
using System.Collections.Generic;
using System.Data;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  // TODO Review 4058: Rename to DataContainerReader and IDataContainerReader
  // TODO Review 4058: Move this class + interface + ObjectFactory + interface to StorageProviderCommands\DataReaders
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into <see cref="DataContainer"/> instances.
  /// </summary>
  public class DataContainerFactory2 : IDataContainerFactory
  {
    private readonly ValueConverter _valueConverter;

    public DataContainerFactory2 (ValueConverter valueConverter)
    {
      ArgumentUtility.CheckNotNull ("valueConverter", valueConverter);

      _valueConverter = valueConverter;
    }

    // TODO Review 4058: Rename to Read
    public virtual DataContainer CreateDataContainer (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read())
        return CreateDataContainerFromReader (dataReader, false);
      else
        return null;
    }

    // TODO Review 4058: Refactor to IEnumerable<DataContainer>; use yield return (instead of List<DataContainer>)
    // TODO Review 4058: Rename to ReadSequence
    public virtual DataContainer[] CreateCollection ( IDataReader dataReader, bool allowNulls)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var collection = new List<DataContainer>();
      var loadedIDs = new HashSet<ObjectID>();

      while (dataReader.Read())
      {
        var dataContainer = CreateDataContainerFromReader (dataReader, allowNulls);
        if (dataContainer != null)
        {
          if (loadedIDs.Contains (dataContainer.ID))
          {
            throw new RdbmsProviderException (
                string.Format ("A database query returned duplicates of the domain object '{0}', which is not supported.", dataContainer.ID));
          }
          loadedIDs.Add (dataContainer.ID);
        }

        collection.Add (dataContainer);
      }

      return collection.ToArray();
    }

    protected virtual DataContainer CreateDataContainerFromReader (IDataReader dataReader, bool allowNulls)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var id = _valueConverter.GetID (dataReader);
      if (id != null)
      {
        var timestamp = _valueConverter.GetTimestamp (dataReader);

        var dataContainer = DataContainer.CreateForExisting (
            id,
            timestamp,
            propertyDefinition => GetDataValue (dataReader, propertyDefinition, id));
        return dataContainer;
      }
      else if (allowNulls)
        return null;
      else
        throw new RdbmsProviderException ("An object returned from the database had a NULL ID, which is not supported.");
    }

    private object GetDataValue (IDataReader dataReader, PropertyDefinition propertyDefinition, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      try
      {
        return _valueConverter.GetValue (objectID.ClassDefinition, propertyDefinition, dataReader);
      }
      catch (Exception e)
      {
        throw new RdbmsProviderException (
            string.Format (
                "Error while reading property '{0}' of object '{1}': {2}",
                propertyDefinition.PropertyName,
                objectID,
                e.Message),
            e);
      }
    }
  }
  
}