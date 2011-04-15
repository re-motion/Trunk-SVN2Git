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
  /// <see cref="FilterViewDefinition"/> defines a filtered view in a relational database.
  /// </summary>
  public class FilterViewDefinition : EntityDefinitionBase, IEntityDefinition
  {
    private readonly IEntityDefinition _baseEntity;
    private readonly ReadOnlyCollection<string> _classIDs;
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly ReadOnlyCollection<IIndexDefinition> _indexes;

    public FilterViewDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IEntityDefinition baseEntity,
        IEnumerable<string> classIDs,
        IEnumerable<IColumnDefinition> columns,
        IEnumerable<IIndexDefinition> indexes) : base(ArgumentUtility.CheckNotNull ("columns", columns), viewName)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);
      ArgumentUtility.CheckNotNullOrEmpty ("classIDs", classIDs);
      ArgumentUtility.CheckNotNull ("indexes", indexes);

      if (!(baseEntity is TableDefinition || baseEntity is FilterViewDefinition))
      {
        throw new ArgumentTypeException (
            "The base entity must either be a TableDefinition or a FilterViewDefinition.",
            "baseEntity",
            null,
            baseEntity.GetType());
      }

      _storageProviderDefinition = storageProviderDefinition;
      _baseEntity = baseEntity;
      _classIDs = classIDs.ToList().AsReadOnly();
      _indexes = indexes.ToList().AsReadOnly();
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public IEntityDefinition BaseEntity
    {
      get { return _baseEntity; }
    }

    public ReadOnlyCollection<string> ClassIDs
    {
      get { return _classIDs; }
    }

    public string LegacyEntityName
    {
      get { return null; }
    }

    public string LegacyViewName
    {
      get { return ViewName.EntityName; }
    }

    public ReadOnlyCollection<IIndexDefinition> Indexes
    {
      get { return _indexes; }
    }

    // Always returns a table
    public TableDefinition GetBaseTable ()
    {
      var current = BaseEntity;
      while (current is FilterViewDefinition)
        current = ((FilterViewDefinition) current).BaseEntity;
      
      return (TableDefinition) current;
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitFilterViewDefinition (this);
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}