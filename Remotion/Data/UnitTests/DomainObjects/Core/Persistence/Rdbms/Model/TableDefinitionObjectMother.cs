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
  public static class TableDefinitionObjectMother
  {
    public static TableDefinition Create (StorageProviderDefinition storageProviderDefinition)
    {
      return Create (storageProviderDefinition, new EntityNameDefinition ("TestSchema", "TestTable"));
    }

    public static TableDefinition Create (StorageProviderDefinition storageProviderDefinition, EntityNameDefinition tableName)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          null);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition, EntityNameDefinition tableName, EntityNameDefinition viewName)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          viewName,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ColumnDefinition objectIdColumnDefinition,
        ColumnDefinition classIdColumnDefinition,
        ColumnDefinition timestampColumnDefinition,
        params ColumnDefinition[] dataColumns)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          viewName,
          objectIdColumnDefinition,
          classIdColumnDefinition,
          timestampColumnDefinition,
          dataColumns,
          new ITableConstraintDefinition[0]);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ColumnDefinition objectIdColumnDefinition,
        ColumnDefinition classIdColumnDefinition,
        ColumnDefinition timestampColumnDefinition,
        IEnumerable<ColumnDefinition> dataColumns,
        ITableConstraintDefinition[] tableConstraintDefinitions)
    {
      return new TableDefinition (
                storageProviderDefinition,
                tableName,
                viewName,
                objectIdColumnDefinition,
                classIdColumnDefinition,
                timestampColumnDefinition,
                dataColumns,
                tableConstraintDefinitions,
                new IIndexDefinition[0],
                new EntityNameDefinition[0]);
    }
  }
}