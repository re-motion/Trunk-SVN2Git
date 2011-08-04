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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="UnionViewDefinition"/> defines a union view in a relational database.
  /// </summary>
  public class UnionViewDefinition : EntityDefinitionBase
  {
    private readonly ReadOnlyCollection<IEntityDefinition> _unionedEntities;
    private readonly StorageProviderDefinition _storageProviderDefinition;

    public UnionViewDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IEnumerable<IEntityDefinition> unionedEntities,
        ColumnDefinition objectIDColumnDefinition,
        ColumnDefinition classIDColumnDefinition,
        ColumnDefinition timstampColumnDefinition,
        IEnumerable<ColumnDefinition> dataColumns,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
        : base (viewName, objectIDColumnDefinition, classIDColumnDefinition, timstampColumnDefinition, dataColumns, indexes, synonyms)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("unionedEntities", unionedEntities);
      ArgumentUtility.CheckNotNull ("dataColumns", dataColumns);
 
      var unionedEntitiesList = unionedEntities.ToList().AsReadOnly();
      for (int i = 0; i < unionedEntitiesList.Count; ++i)
      {
        var unionedEntity = unionedEntitiesList[i];
        if (!(unionedEntity is TableDefinition || unionedEntity is UnionViewDefinition))
        {
          throw new ArgumentItemTypeException (
              "unionedEntities",
              i,
              null,
              unionedEntity.GetType(),
              "The unioned entities must either be a TableDefinitions or UnionViewDefinitions.");
        }
      }

      _storageProviderDefinition = storageProviderDefinition;
      _unionedEntities = unionedEntitiesList;
      indexes.ToList().AsReadOnly();
    }

    public override string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public override StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public ReadOnlyCollection<IEntityDefinition> UnionedEntities
    {
      get { return _unionedEntities; }
    }

    public override string LegacyEntityName
    {
      get { return null; }
    }

    public ColumnDefinition[] CreateFullColumnList (IEnumerable<ColumnDefinition> availableColumns)
    {
      ArgumentUtility.CheckNotNull ("availableColumns", availableColumns);

      // TODO 4213: Currently, there is one possibility of duplicate columns for the same property being created: when a mixin adds a property 
      // to more than  one subclass of the same type, the corresponding ColumnDefinition will be created twice. Therefore, we can't simply match 
      // ColumnDefinition references - as we'd like to -, but have to check for ordinals by name.
      var availableColumnsAsDictionary = availableColumns.ToDictionary (c => c.Name);

      return GetAllColumns().Select (columnDefinition => availableColumnsAsDictionary.GetValueOrDefault (columnDefinition.Name)).ToArray();
    }

    // Always returns at least one table
    public IEnumerable<TableDefinition> GetAllTables ()
    {
      foreach (var entityDefinition in _unionedEntities)
      {
        if (entityDefinition is TableDefinition)
          yield return (TableDefinition) entityDefinition;
        else
        {
          foreach (var derivedTable in ((UnionViewDefinition) entityDefinition).GetAllTables())
            yield return derivedTable;
        }
      }
    }

    public override void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitUnionViewDefinition (this);
    }

    public override bool IsNull
    {
      get { return false; }
    }
  }
}