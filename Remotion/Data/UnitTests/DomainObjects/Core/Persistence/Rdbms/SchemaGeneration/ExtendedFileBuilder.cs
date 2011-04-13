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

      script.Insert (0, "CREATE SCHEMA Test\r\nGO\r\n");
      script.Insert (0, "--Extendend file-builder comment at the beginning\r\n");
      script.AppendLine ("DROP SCHEMA Test");
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
            new EntityNameDefinition ("Test", "NewTableName"),
            firstTableDefinition.ViewName,
            firstTableDefinition.GetColumns(),
            firstTableDefinition.Constraints,
            new IIndexDefinition[0]);
        entityDefinitions.Remove (firstTableDefinition);
        entityDefinitions.Add (newTableDefinition);

        var newFilterViewDefinition = new FilterViewDefinition (
            firstTableDefinition.StorageProviderDefinition,
            new EntityNameDefinition ("Test", "AddedView"),
            firstTableDefinition,
            new[] { "ClassID" },
            firstTableDefinition.GetColumns(),
            new IIndexDefinition[0]);
        entityDefinitions.Add (newFilterViewDefinition);
      }

      return entityDefinitions;
    }
  }
}