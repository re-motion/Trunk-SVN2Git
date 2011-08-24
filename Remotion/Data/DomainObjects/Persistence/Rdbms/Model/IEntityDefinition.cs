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
using Remotion.Data.DomainObjects.Persistence.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="IEntityDefinition"/> defines the API for an entity definition for a relational database.
  /// </summary>
  public interface IEntityDefinition : IStorageEntityDefinition, INullObject
  {
    EntityNameDefinition ViewName { get; }
    ReadOnlyCollection<IIndexDefinition> Indexes { get; }
    ReadOnlyCollection<EntityNameDefinition> Synonyms { get; }
    
    ColumnDefinition IDColumn { get; }
    ColumnDefinition ClassIDColumn { get; }
    ColumnDefinition TimestampColumn { get; }
    IEnumerable<ColumnDefinition> DataColumns { get; }

    ObjectIDStoragePropertyDefinition ObjectIDProperty { get; }
    IRdbmsStoragePropertyDefinition TimestampProperty { get; }
    IEnumerable<IRdbmsStoragePropertyDefinition> DataProperties { get; }

    IEnumerable<IRdbmsStoragePropertyDefinition> GetAllProperties ();
    IEnumerable<ColumnDefinition> GetAllColumns ();
    void Accept (IEntityDefinitionVisitor visitor);
  }
}