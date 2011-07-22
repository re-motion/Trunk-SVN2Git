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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into <see cref="DataContainer"/> instances.
  /// </summary>
  public class DataContainerReader : IObjectReader<DataContainer>
  {
    private readonly IRdbmsStoragePropertyDefinition _idProperty;
    private readonly IRdbmsStoragePropertyDefinition _timestampProperty;
    private readonly IColumnOrdinalProvider _ordinalProvider;
    private readonly IRdbmsPersistenceModelProvider _persistenceModelProvider;

    public DataContainerReader (
        IRdbmsStoragePropertyDefinition idProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IColumnOrdinalProvider ordinalProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("idProperty", idProperty);
      ArgumentUtility.CheckNotNull ("timestampProperty", timestampProperty);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);

      _idProperty = idProperty;
      _timestampProperty = timestampProperty;
      _ordinalProvider = ordinalProvider;
      _persistenceModelProvider = persistenceModelProvider;
    }
    
    public IRdbmsStoragePropertyDefinition IDProperty
    {
      get { return _idProperty; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return _timestampProperty; }
    }

    public IColumnOrdinalProvider OrdinalProvider
    {
      get { return _ordinalProvider; }
    }

    public IRdbmsPersistenceModelProvider PersistenceModelProvider
    {
      get { return _persistenceModelProvider; }
    }

    public virtual DataContainer Read (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read())
        return CreateDataContainerFromReader (dataReader);
      else
        return null;
    }

    public virtual IEnumerable<DataContainer> ReadSequence (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      while (dataReader.Read())
        yield return CreateDataContainerFromReader (dataReader);
    }

    protected virtual DataContainer CreateDataContainerFromReader (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var id = (ObjectID) _idProperty.Read (dataReader, _ordinalProvider);
      if (id == null)
        return null;

      var timestamp = _timestampProperty.Read (dataReader, _ordinalProvider);
      return DataContainer.CreateForExisting (id, timestamp, pd => ReadPropertyValue (pd, dataReader, id));
    }

    private object ReadPropertyValue (PropertyDefinition propertyDefinition, IDataReader dataReader, ObjectID id)
    {
      try
      {
        var storagePropertyDefinition = _persistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
        return storagePropertyDefinition.Read (dataReader, _ordinalProvider) ?? propertyDefinition.DefaultValue;
      }
      catch (Exception e)
      {
        var message = string.Format ("Error while reading property '{0}' of object '{1}': {2}", propertyDefinition.PropertyName, id, e.Message);
        throw new RdbmsProviderException (message, e);
      }
    }
  }
}