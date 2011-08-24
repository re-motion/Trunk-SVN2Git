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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  public static class FilterViewDefinitionObjectMother
  {
    public static FilterViewDefinition Create (StorageProviderDefinition storageProviderDefinition)
    {
      return Create (storageProviderDefinition, new EntityNameDefinition ("TestSchema", "Test"));
    }

    public static FilterViewDefinition Create (StorageProviderDefinition storageProviderDefinition, EntityNameDefinition viewName)
    {
      return Create (storageProviderDefinition, viewName, TableDefinitionObjectMother.Create (storageProviderDefinition));
    }

    public static FilterViewDefinition Create (
        StorageProviderDefinition storageProviderDefinition, EntityNameDefinition viewName, IEntityDefinition baseEntity)
    {
      return Create (
          storageProviderDefinition,
          viewName,
          baseEntity,
          new[] { "ClassID1" },
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0]);
    }

    public static FilterViewDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IEntityDefinition baseEntity,
        IEnumerable<string> classIDs,
        ColumnDefinition objectIDColumnDefinition,
        ColumnDefinition classIDColumnDefinition,
        ColumnDefinition timstampColumnDefinition,
        IEnumerable<ColumnDefinition> dataColumns)
    {
      return new FilterViewDefinition (
          storageProviderDefinition,
          viewName,
          baseEntity,
          classIDs,
          objectIDColumnDefinition,
          classIDColumnDefinition,
          timstampColumnDefinition,
          dataColumns,
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    public static FilterViewDefinition CreateWithIndexes (StorageProviderDefinition storageProviderDefinition, IEnumerable<IIndexDefinition> indexDefinitions)
    {
      return new FilterViewDefinition (
          storageProviderDefinition,
          new EntityNameDefinition ("TestSchema", "TestFilterView"),
          TableDefinitionObjectMother.Create (storageProviderDefinition),
          new[] { "Class1" },
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0],
          indexDefinitions,
          new EntityNameDefinition[0]);
    }

    public static FilterViewDefinition CreateWithSynonyms (StorageProviderDefinition storageProviderDefinition, IEnumerable<EntityNameDefinition> synonyms)
    {
      return new FilterViewDefinition (
          storageProviderDefinition,
          new EntityNameDefinition ("TestSchema", "TestFilterView"),
          TableDefinitionObjectMother.Create (storageProviderDefinition),
          new[] { "Class1" },
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0],
          new IIndexDefinition[0],
          synonyms);
    }
  }
}