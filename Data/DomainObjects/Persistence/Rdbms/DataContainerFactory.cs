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
using System.Collections.Generic;
using System.Data;
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
        return CreateDataContainerFromReader ();
      else
        return null;
    }

    public virtual DataContainerCollection CreateCollection ()
    {
      DataContainerCollection dataContainerCollection = new DataContainerCollection ();

      while (_dataReader.Read ())
      {
        DataContainer dataContainer = CreateDataContainerFromReader ();
        if (dataContainerCollection.Contains (dataContainer.ID))
          throw _provider.CreateRdbmsProviderException (
              "A database query returned duplicates of the domain object '{0}', which is not supported.",
              dataContainer.ID);
        dataContainerCollection.Add (dataContainer);
      }

      return dataContainerCollection;
    }

    protected virtual DataContainer CreateDataContainerFromReader ()
    {
      ValueConverter valueConverter = _provider.CreateValueConverter ();
      
      ObjectID id = valueConverter.GetID (_dataReader);
      if (id == null)
        throw _provider.CreateRdbmsProviderException ("An object returned from the database had a NULL ID, which is not supported.");

      object timestamp = _dataReader.GetValue (valueConverter.GetMandatoryOrdinal (_dataReader, "Timestamp"));

      DataContainer dataContainer = DataContainer.CreateForExisting (id, timestamp, 
          delegate (PropertyDefinition propertyDefinition) { return GetDataValue (propertyDefinition, id, valueConverter); });
      return dataContainer;
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
