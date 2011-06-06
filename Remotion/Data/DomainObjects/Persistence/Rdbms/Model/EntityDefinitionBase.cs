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
  /// <see cref="EntityDefinitionBase"/> is the base-class for all entity definitions.
  /// </summary>
  public abstract class EntityDefinitionBase : IEntityDefinition
  {
    private readonly EntityNameDefinition _viewName;
    private readonly ReadOnlyCollection<EntityNameDefinition> _synonyms;
    private readonly SimpleColumnDefinition _objectIDColumn;
    private readonly SimpleColumnDefinition _classIDColumn;
    private readonly SimpleColumnDefinition _timestampColumn;
    private readonly ReadOnlyCollection<SimpleColumnDefinition> _columns;

    protected EntityDefinitionBase (
        EntityNameDefinition viewName,
        SimpleColumnDefinition objectIDColumn,
        SimpleColumnDefinition classIDColumn,
        SimpleColumnDefinition timstampColumn,
        IEnumerable<SimpleColumnDefinition> columns,
        IEnumerable<EntityNameDefinition> synonyms)
    {
      ArgumentUtility.CheckNotNull ("objectIDColumn", objectIDColumn);
      ArgumentUtility.CheckNotNull ("classIDColumn", classIDColumn);
      ArgumentUtility.CheckNotNull ("timstampColumn", timstampColumn);
      ArgumentUtility.CheckNotNull ("columns", columns);
      ArgumentUtility.CheckNotNull ("synonyms", synonyms);

      _viewName = viewName;
      _objectIDColumn = objectIDColumn;
      _classIDColumn = classIDColumn;
      _timestampColumn = timstampColumn;
      _columns = columns.ToList().AsReadOnly();
      _synonyms = synonyms.ToList().AsReadOnly();
    }

    public abstract string LegacyEntityName { get; }

    public abstract string StorageProviderID { get; }

    public abstract StorageProviderDefinition StorageProviderDefinition { get; }

    public abstract ReadOnlyCollection<IIndexDefinition> Indexes { get; }

    public abstract bool IsNull { get; }

    public abstract void Accept (IEntityDefinitionVisitor visitor);

    public EntityNameDefinition ViewName
    {
      get { return _viewName; }
    }

    public SimpleColumnDefinition ObjectIDColumn
    {
      get { return _objectIDColumn; }
    }

    public SimpleColumnDefinition ClassIDColumn
    {
      get { return _classIDColumn; }
    }

    public SimpleColumnDefinition TimestampColumn
    {
      get { return _timestampColumn; }
    }

    public IEnumerable<SimpleColumnDefinition> DataColumns
    {
      get { return _columns; }
    }

    public IEnumerable<SimpleColumnDefinition> GetAllColumns ()
    {
      yield return _objectIDColumn;
      yield return _classIDColumn;
      yield return _timestampColumn;

      foreach (var column in _columns)
        yield return column;
    }

    public ReadOnlyCollection<EntityNameDefinition> Synonyms
    {
      get { return _synonyms; }
    }

    public string LegacyViewName
    {
      get { return _viewName.EntityName; }
    }
  }
}