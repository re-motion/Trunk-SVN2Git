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
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// The <see cref="ObjectReaderFactory"/> is responsible to create <see cref="IObjectReader{T}"/> instances.
  /// </summary>
  public class ObjectReaderFactory : IObjectReaderFactory
  {
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;

    public ObjectReaderFactory (
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
    }

    public IObjectReader<DataContainer> CreateDataContainerReader ()
    {
      var ordinalProvider = new NameBasedColumnOrdinalProvider ();
      var objectIDStoragePropertyDefinition = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition();
      var timestampPropertyDefinition = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition();
      return new DataContainerReader (
          objectIDStoragePropertyDefinition, timestampPropertyDefinition, ordinalProvider, _rdbmsPersistenceModelProvider);
    }

    public IObjectReader<DataContainer> CreateDataContainerReader (IRdbmsStorageEntityDefinition entityDefinition, IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      var ordinalProvider = CreateOrdinalProviderForKnownProjection (selectedColumns);
      return new DataContainerReader (
          entityDefinition.ObjectIDProperty, 
          entityDefinition.TimestampProperty, 
          ordinalProvider, 
          _rdbmsPersistenceModelProvider);
    }

    public IObjectReader<ObjectID> CreateObjectIDReader (IRdbmsStorageEntityDefinition entityDefinition, IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      var ordinalProvider = CreateOrdinalProviderForKnownProjection (selectedColumns);
      return new ObjectIDReader (entityDefinition.ObjectIDProperty, ordinalProvider);
    }

    public IObjectReader<Tuple<ObjectID, object>> CreateTimestampReader (
        IRdbmsStorageEntityDefinition entityDefinition, IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      var ordinalProvider = CreateOrdinalProviderForKnownProjection (selectedColumns);
      return new TimestampReader (entityDefinition.ObjectIDProperty, entityDefinition.TimestampProperty, ordinalProvider);
    }

    private IColumnOrdinalProvider CreateOrdinalProviderForKnownProjection (IEnumerable<ColumnDefinition> selectedColumns)
    {
      var columnOrdinalsDictionary = selectedColumns.Select ((column, index) => new { column, index }).ToDictionary (t => t.column, t => t.index);
      return new DictionaryBasedColumnOrdinalProvider (columnOrdinalsDictionary);
    }
  }
}