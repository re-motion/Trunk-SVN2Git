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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ExtendedEntityDefinitionProvider : IEntityDefinitionProvider
  {
    public IEnumerable<IEntityDefinition> GetEntityDefinitions (IEnumerable<ClassDefinition> classDefinitions)
    {
      var entityDefinitions = new EntityDefinitionProvider().GetEntityDefinitions (classDefinitions).ToList ();

      var tableDefinitions = entityDefinitions.OfType<TableDefinition> ().ToList ();
      if (tableDefinitions.Count () > 0)
      {
        var firstTableDefinition = tableDefinitions[0];
        var newTableDefinition = new TableDefinition (
            firstTableDefinition.StorageProviderDefinition,
            firstTableDefinition.TableName,
            new EntityNameDefinition (firstTableDefinition.ViewName.SchemaName, "NewViewName"),
            firstTableDefinition.ObjectIDColumn,
            firstTableDefinition.ClassIDColumn,
            firstTableDefinition.TimestampColumn,
            firstTableDefinition.DataColumns,
            firstTableDefinition.Constraints,
            new IIndexDefinition[0],
            new EntityNameDefinition[0]);
        entityDefinitions.Remove (firstTableDefinition);
        entityDefinitions.Add (newTableDefinition);

        entityDefinitions.Add (CreateNewFilterViewDefinition (newTableDefinition));
        entityDefinitions.Add (CreateNewTableDefinitionWithIndexes (newTableDefinition.StorageProviderDefinition));
        entityDefinitions.Add (CreateNewTableDefinitionWithNonClusteredPrimaryKey (newTableDefinition.StorageProviderDefinition));
      }

      return entityDefinitions;
    }



    private FilterViewDefinition CreateNewFilterViewDefinition (TableDefinition tableDefinition)
    {
      return new FilterViewDefinition (
          tableDefinition.StorageProviderDefinition,
          new EntityNameDefinition (tableDefinition.TableName.SchemaName, "AddedView"),
          tableDefinition,
          new[] { "ClassID" },
          tableDefinition.ObjectIDColumn,
          tableDefinition.ClassIDColumn,
          tableDefinition.TimestampColumn,
          tableDefinition.DataColumns,
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition (tableDefinition.ViewName.SchemaName, "AddedViewSynonym") });
    }

    private TableDefinition CreateNewTableDefinitionWithIndexes (StorageProviderDefinition storageProviderDefinition)
    {
      var column1 = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      var column2 = new SimpleColumnDefinition ("FirstName", typeof (string), "varchar(100)", false, false);
      var column3 = new SimpleColumnDefinition ("LastName", typeof (string), "varchar(100)", false, false);
      var column4 = new SimpleColumnDefinition ("XmlColumn1", typeof (string), "xml", false, false);

      var tableName = new EntityNameDefinition (null, "IndexTestTable");
      var viewName = new EntityNameDefinition (null, "IndexTestView");

      var nonClusteredUniqueIndex = new SqlIndexDefinition (
          "IDX_NonClusteredUniqueIndex", new[] { new SqlIndexedColumnDefinition (column1) }, null, false, true, true, false);
      var nonClusteredNonUniqueIndex = new SqlIndexDefinition (
          "IDX_NonClusteredNonUniqueIndex",
          new[] { new SqlIndexedColumnDefinition (column2), new SqlIndexedColumnDefinition (column3) },
          new[] { column1 },
          false,
          false,
          false,
          false);
      var indexWithOptionsSet = new SqlIndexDefinition (
          "IDX_IndexWithSeveralOptions",
          new[] { new SqlIndexedColumnDefinition (column2, IndexOrder.Desc) },
          null,
          false,
          true,
          true,
          false,
          true,
          5,
          true,
          true,
          false,
          true,
          true,
          2);
      var primaryXmlIndex = new SqlPrimaryXmlIndexDefinition ("IDX_PrimaryXmlIndex", column4, true, 3, true, true, false, true, true, 2);
      var secondaryXmlIndex1 = new SqlSecondaryXmlIndexDefinition (
          "IDX_SecondaryXmlIndex1",
          column4,
          "IDX_PrimaryXmlIndex",
          SqlSecondaryXmlIndexKind.Path,
          true,
          null,
          false,
          false,
          false,
          false,
          false);
      var secondaryXmlIndex2 = new SqlSecondaryXmlIndexDefinition (
          "IDX_SecondaryXmlIndex2", column4, "IDX_PrimaryXmlIndex", SqlSecondaryXmlIndexKind.Value, false, 8, true);
      var secondaryXmlIndex3 = new SqlSecondaryXmlIndexDefinition (
          "IDX_SecondaryXmlIndex3",
          column4,
          "IDX_PrimaryXmlIndex",
          SqlSecondaryXmlIndexKind.Property,
          null,
          null,
          null,
          true,
          null,
          true,
          false);

      var objectIDColunmn = new SimpleColumnDefinition ("ObjectID", typeof (int), "integer", false, true);
      var classIDCOlumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var timestampColumn = new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);

      return new TableDefinition (
          storageProviderDefinition,
          tableName,
          viewName,
          objectIDColunmn,
          classIDCOlumn,
          timestampColumn,
          new[] { column1, column2, column3, column4 },
          new[] { new PrimaryKeyConstraintDefinition ("PK_IndexTestTable_ID", true, new[] { column1 }) },
          new IIndexDefinition[]
          {
              nonClusteredUniqueIndex,
              nonClusteredNonUniqueIndex,
              indexWithOptionsSet,
              primaryXmlIndex,
              secondaryXmlIndex1,
              secondaryXmlIndex2,
              secondaryXmlIndex3
          },
          new EntityNameDefinition[0]);
    }

    private TableDefinition CreateNewTableDefinitionWithNonClusteredPrimaryKey (StorageProviderDefinition storageProviderDefinition)
    {
      var tableName = new EntityNameDefinition (null, "PKTestTable");
      var viewName = new EntityNameDefinition (null, "PKTestView");

      var column1 = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      var column2 = new SimpleColumnDefinition ("Name", typeof (string), "varchar(100)", false, false);
      var objectIDColunmn = new SimpleColumnDefinition ("ObjectID", typeof (int), "integer", false, true);
      var classIDCOlumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var timestampColumn = new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);

      var nonClusteredUniqueIndex = new SqlIndexDefinition (
          "IDX_ClusteredUniqueIndex", new[] { new SqlIndexedColumnDefinition (column2) }, null, true, true, true, false);

      return new TableDefinition (
          storageProviderDefinition,
          tableName,
          viewName,
          objectIDColunmn,
          classIDCOlumn,
          timestampColumn,
          new[] { column1, column2 },
          new[] { new PrimaryKeyConstraintDefinition ("PK_PKTestTable_ID", false, new[] { column1 }) },
          new IIndexDefinition[] { nonClusteredUniqueIndex },
          new EntityNameDefinition[0]);
    }
  }
}