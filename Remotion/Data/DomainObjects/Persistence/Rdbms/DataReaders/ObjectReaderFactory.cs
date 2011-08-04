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

    public ObjectReaderFactory (IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public IObjectReader<DataContainer> CreateDataContainerReader (IEntityDefinition entityDefinition, IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      var ordinalProvider = CreateOrdinalProviderForKnownProjection (selectedColumns);
      var objectIDStoragePropertyDefinition = InfrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition (entityDefinition);
      var timestampPropertyDefinition = InfrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition (entityDefinition);
      return new DataContainerReader (objectIDStoragePropertyDefinition, timestampPropertyDefinition, ordinalProvider, _rdbmsPersistenceModelProvider);
    }

    public IObjectReader<ObjectID> CreateObjectIDReader (IEntityDefinition entityDefinition, IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      var ordinalProvider = CreateOrdinalProviderForKnownProjection (selectedColumns);
      var objectIDStoragePropertyDefinition = InfrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition (entityDefinition);
      return new ObjectIDReader (objectIDStoragePropertyDefinition, ordinalProvider);
    }

    public IObjectReader<Tuple<ObjectID, object>> CreateTimestampReader (
        IEntityDefinition entityDefinition, IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      var ordinalProvider = CreateOrdinalProviderForKnownProjection (selectedColumns);
      var objectIDStoragePropertyDefinition = InfrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition (entityDefinition);
      var timestampPropertyDefinition = InfrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition (entityDefinition);
      return new TimestampReader (objectIDStoragePropertyDefinition, timestampPropertyDefinition, ordinalProvider);
    }

    private IColumnOrdinalProvider CreateOrdinalProviderForKnownProjection (IEnumerable<ColumnDefinition> selectedColumns)
    {
      // TODO 4213: Currently, there is one possibility of duplicate columns for the same property being created: when a mixin adds a property 
      // to more than  one subclass of the same type, the corresponding ColumnDefinition will be created twice. Therefore, we can't simply match 
      // ColumnDefinition references - as we'd like to -, but have to check for ordinals by name.
      var columnOrdinalsDictionary = selectedColumns.Select ((column, index) => new { column, index }).ToDictionary (t => t.column.Name, t => t.index);
      return new DictionaryBasedColumnOrdinalProvider (columnOrdinalsDictionary);
    }
  }
}