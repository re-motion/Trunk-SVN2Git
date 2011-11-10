// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
  public class FilterViewDefinition : RdbmsStorageEntityDefinitionBase
  {
    private readonly IRdbmsStorageEntityDefinition _baseEntity;
    private readonly ReadOnlyCollection<string> _classIDs;

    public FilterViewDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IRdbmsStorageEntityDefinition baseEntity,
        IEnumerable<string> classIDs,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
        : base (
            storageProviderDefinition,
            viewName,
            objectIDProperty,
            timestampProperty,
            dataProperties,
            indexes,
            synonyms)
    {
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);
      ArgumentUtility.CheckNotNullOrEmpty ("classIDs", classIDs);

      if (!(baseEntity is TableDefinition || baseEntity is FilterViewDefinition))
      {
        throw new ArgumentTypeException (
            "The base entity must either be a TableDefinition or a FilterViewDefinition.",
            "baseEntity",
            null,
            baseEntity.GetType());
      }

      _baseEntity = baseEntity;
      _classIDs = classIDs.ToList().AsReadOnly();
    }

    public IRdbmsStorageEntityDefinition BaseEntity
    {
      get { return _baseEntity; }
    }

    public ReadOnlyCollection<string> ClassIDs
    {
      get { return _classIDs; }
    }

    // Always returns a table
    public TableDefinition GetBaseTable ()
    {
      var current = BaseEntity;
      while (current is FilterViewDefinition)
        current = ((FilterViewDefinition) current).BaseEntity;

      return (TableDefinition) current;
    }

    public override void Accept (IRdbmsStorageEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitFilterViewDefinition (this);
    }
  }
}