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
    private readonly ReadOnlyCollection<IIndexDefinition> _indexes;

    public UnionViewDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IEnumerable<IEntityDefinition> unionedEntities,
        IEnumerable<IColumnDefinition> columns,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
        : base (viewName, ArgumentUtility.CheckNotNull ("columns", columns), ArgumentUtility.CheckNotNull ("synonyms", synonyms))
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("unionedEntities", unionedEntities);
      ArgumentUtility.CheckNotNull ("columns", columns);
      ArgumentUtility.CheckNotNull ("indexes", indexes);

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
      _indexes = indexes.ToList().AsReadOnly();
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

    public override ReadOnlyCollection<IIndexDefinition> Indexes
    {
      get { return _indexes; }
    }

    public IColumnDefinition[] CreateFullColumnList (IEnumerable<IColumnDefinition> availableColumns)
    {
      ArgumentUtility.CheckNotNull ("availableColumns", availableColumns);

      var finder = new ColumnDefinitionFinder (availableColumns);
      return Columns.Select (finder.FindColumn).ToArray();
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