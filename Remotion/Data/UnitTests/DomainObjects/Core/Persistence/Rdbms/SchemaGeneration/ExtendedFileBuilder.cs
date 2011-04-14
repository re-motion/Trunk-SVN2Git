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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  public class ExtendedFileBuilder : FileBuilder
  {
    public ExtendedFileBuilder (ScriptBuilderBase scriptBuilder)
        : base (scriptBuilder)
    {
    }

    public override string GetScript (IEnumerable<ClassDefinition> classDefinitions)
    {
      var script = new StringBuilder (base.GetScript (classDefinitions));

      script.Insert (0, "IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END\r\nGO\r\n");
      script.Insert (0, "--Extendend file-builder comment at the beginning\r\n");
      script.AppendLine ("--Extendend file-builder comment at the end");

      return script.ToString();
    }

    protected override IEnumerable<IEntityDefinition> GetEntityDefinitions (IEnumerable<ClassDefinition> classDefinitions)
    {
      var entityDefinitions = base.GetEntityDefinitions (classDefinitions).ToList();

      var tableDefinitions = entityDefinitions.OfType<TableDefinition>().ToList();
      if (tableDefinitions.Count() > 0)
      {
        var firstTableDefinition = tableDefinitions[0];
        var newTableDefinition = new TableDefinition (
            firstTableDefinition.StorageProviderDefinition,
            firstTableDefinition.TableName,
            new EntityNameDefinition (firstTableDefinition.ViewName.SchemaName, "NewViewName"),
            firstTableDefinition.GetColumns(),
            firstTableDefinition.Constraints,
            new IIndexDefinition[0]);
        entityDefinitions.Remove (firstTableDefinition);
        entityDefinitions.Add (newTableDefinition);

        entityDefinitions.Add (CreateNewFilterViewDefinition(newTableDefinition));
        entityDefinitions.Add (CreateNewTableDefinitionWithIndexes (newTableDefinition.StorageProviderDefinition));
      }

      return entityDefinitions;
    }

    private FilterViewDefinition CreateNewFilterViewDefinition (TableDefinition tableDefinition)
    {
      return new FilterViewDefinition (
            tableDefinition.StorageProviderDefinition,
            new EntityNameDefinition ("Test", "AddedView"),
            tableDefinition,
            new[] { "ClassID" },
            tableDefinition.GetColumns (),
            new IIndexDefinition[0]);
    }

    private TableDefinition CreateNewTableDefinitionWithIndexes (StorageProviderDefinition storageProviderDefinition)
    {
      var column1 = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      var column2 = new SimpleColumnDefinition ("FirstName", typeof (string), "varchar(100)", false, false);
      var column3 = new SimpleColumnDefinition ("LastName", typeof (string), "varchar(100)", false, false);
      var column4 = new SimpleColumnDefinition ("XmlColumn1", typeof (string), "xml", false, false);
      var column5 = new SimpleColumnDefinition ("XmlColumn2", typeof (string), "xml", true, false);
      var column6 = new SimpleColumnDefinition ("XmlColumn3", typeof (string), "xml", true, false);
      var column7 = new SimpleColumnDefinition ("XmlColumn4", typeof (string), "xml", true, false);

      var tableName = new EntityNameDefinition (null, "IndexTestTable");
      var viewName = new EntityNameDefinition (null, "IndexTestView");

      var clusteredUniqueIndex = new IndexDefinition ("IDX_ClusteredUniqueIndex", tableName, new[] { column1 }, null, true, true, true, true);

      return new TableDefinition (
          storageProviderDefinition,
          tableName,
          viewName,
          new[] { column1, column2, column3, column4, column5, column6, column7 },
          new ITableConstraintDefinition[0],
          new IIndexDefinition[] { clusteredUniqueIndex });
    }
  }
}