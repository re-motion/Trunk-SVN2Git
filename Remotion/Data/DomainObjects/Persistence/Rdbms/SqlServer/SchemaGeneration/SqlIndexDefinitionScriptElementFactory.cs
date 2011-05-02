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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlIndexDefinitionScriptElementFactory"/> is responsible to create script-elements for standard indexes in a sql-server database.
  /// </summary>
  public class SqlIndexDefinitionScriptElementFactory : SqlIndexScriptElementFactoryBase<SqlIndexDefinition>
  {
    public override IScriptElement GetCreateElement (SqlIndexDefinition indexDefinition)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      return new ScriptStatement(
      string.Format (
         "CREATE {0}{1} INDEX [{2}]\r\n"
         + "  ON [{3}].[{4}] ({5}){6}{7}",
         indexDefinition.IsUnique.HasValue && indexDefinition.IsUnique.Value ? "UNIQUE " : string.Empty,
         indexDefinition.IsClustered.HasValue && indexDefinition.IsClustered.Value ? "CLUSTERED" : "NONCLUSTERED",
         indexDefinition.IndexName,
         indexDefinition.ObjectName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
         indexDefinition.ObjectName.EntityName,
         GetIndexedColumnNames (indexDefinition.Columns),
         indexDefinition.IncludedColumns != null
             ? "\r\n  INCLUDE (" + GetColumnList (indexDefinition.IncludedColumns) + ")"
             : string.Empty,
         GetCreateIndexOptions (GetCreateIndexOptionItems (indexDefinition))));
    }

    protected override IEnumerable<string> GetCreateIndexOptionItems (SqlIndexDefinitionBase indexDefinition)
    {
      var indexDefinitionAsIndexDefinition = indexDefinition as SqlIndexDefinition;
      if (indexDefinitionAsIndexDefinition != null)
      {
        if (indexDefinitionAsIndexDefinition.IgnoreDupKey.HasValue)
          yield return string.Format ("IGNORE_DUP_KEY = {0}", indexDefinitionAsIndexDefinition.IgnoreDupKey.Value ? "ON" : "OFF");
        if (indexDefinitionAsIndexDefinition.Online.HasValue)
          yield return string.Format ("ONLINE = {0}", indexDefinitionAsIndexDefinition.Online.Value ? "ON" : "OFF");
      }

      foreach (var option in base.GetCreateIndexOptionItems (indexDefinition).ToList())
        yield return option;
    }
  }
}