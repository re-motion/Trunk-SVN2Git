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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class DataContainerFactory
  {
    private readonly IDataReader _dataReader;
    private readonly RdbmsProvider _provider;

    public DataContainerFactory (RdbmsProvider provider, IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      _dataReader = dataReader;
      _provider = provider;
    }

    public virtual DataContainer CreateDataContainer ()
    {
      if (_dataReader.Read ())
        return CreateDataContainerFromReader (false);
      else
        return null;
    }

    public virtual DataContainer[] CreateCollection (bool allowNulls)
    {
      var collection = new List<DataContainer> ();
      var loadedIDs = new HashSet<ObjectID> ();

      while (_dataReader.Read ())
      {
        DataContainer dataContainer = CreateDataContainerFromReader (allowNulls);
        if (dataContainer != null)
        {
          if (loadedIDs.Contains (dataContainer.ID))
          {
            throw _provider.CreateRdbmsProviderException (
                "A database query returned duplicates of the domain object '{0}', which is not supported.",
                dataContainer.ID);
          }
          loadedIDs.Add (dataContainer.ID);
        }

        collection.Add (dataContainer);
      }

      return collection.ToArray();
    }

    protected virtual DataContainer CreateDataContainerFromReader (bool allowNulls)
    {
      ValueConverter valueConverter = _provider.CreateValueConverter ();
      
      ObjectID id = valueConverter.GetID (_dataReader);
      if (id != null)
      {
        object timestamp = _dataReader.GetValue (valueConverter.GetMandatoryOrdinal (_dataReader, "Timestamp"));

        DataContainer dataContainer = DataContainer.CreateForExisting (
            id,
            timestamp,
            propertyDefinition => GetDataValue (propertyDefinition, id, valueConverter));
        return dataContainer;
      }
      else if (allowNulls)
        return null;
      else
        throw _provider.CreateRdbmsProviderException ("An object returned from the database had a NULL ID, which is not supported.");
    }

    private object GetDataValue (PropertyDefinition propertyDefinition, ObjectID objectID, ValueConverter valueConverter)
    {
      try
      {
        return valueConverter.GetValue (objectID.ClassDefinition, propertyDefinition, _dataReader);
      }
      catch (Exception e)
      {
        throw _provider.CreateRdbmsProviderException (e, "Error while reading property '{0}' of object '{1}': {2}",
            propertyDefinition.PropertyName, objectID, e.Message);
      }
    }
  }
}
