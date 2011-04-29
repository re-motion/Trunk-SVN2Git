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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlTableViewScriptElementFactory"/> is responsible to create script-elements for filter-views in a sql-server database.
  /// </summary>
  public class SqlFilterViewScriptElementFactory : SqlViewScriptElementFactoryBase<FilterViewDefinition>
  {
    public override IScriptElement GetCreateElement (FilterViewDefinition filterViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

      var statements = new ScriptElementCollection();
      statements.AddElement (new BatchDelimiterStatement());
      statements.AddElement(new ScriptStatement(
       string.Format (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  {3}AS\r\n"
          + "  SELECT {2}\r\n"
          + "    FROM [{4}].[{5}]\r\n"
          + "    WHERE [ClassID] IN ({6})\r\n"
          + "  WITH CHECK OPTION",
          filterViewDefinition.ViewName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
          filterViewDefinition.ViewName.EntityName,
          GetColumnList (filterViewDefinition.Columns),
          UseSchemaBinding (filterViewDefinition) ? "WITH SCHEMABINDING " : string.Empty,
          filterViewDefinition.GetBaseTable ().TableName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
          filterViewDefinition.GetBaseTable ().TableName.EntityName,
          GetClassIDList (filterViewDefinition.ClassIDs))));
      statements.AddElement (new BatchDelimiterStatement());
      return statements;
    }

    protected string GetClassIDList (IEnumerable<string> classIDs)
    {
      return SeparatedStringBuilder.Build (", ", classIDs, id => "'" + id + "'");
    }
  }
}