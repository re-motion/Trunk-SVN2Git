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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlUnionViewScriptElementFactory"/> is responsible to create script-elements for union-views in a sql-server database.
  /// </summary>
  public class SqlUnionViewScriptElementFactory : SqlViewScriptElementFactoryBase<UnionViewDefinition>
  {
    public override IScriptElement GetCreateElement (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      var statements = new ScriptElementCollection ();
      statements.AddElement (CreateBatchDelimiterStatement());

      var createViewStringBuilder = new StringBuilder();
      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  {3}AS\r\n",
          unionViewDefinition.ViewName.SchemaName ?? DefaultSchema,
          unionViewDefinition.ViewName.EntityName,
          GetColumnList (unionViewDefinition.Columns),
          UseSchemaBinding (unionViewDefinition) ? "WITH SCHEMABINDING " : string.Empty);

      int numberOfSelects = 0;
      foreach (var tableDefinition in unionViewDefinition.GetAllTables ())
      {
        if (numberOfSelects > 0)
          createViewStringBuilder.AppendFormat ("\r\n  UNION ALL\r\n");

        var availableTableColumns = tableDefinition.Columns;
        var unionedColumns = unionViewDefinition.CreateFullColumnList (availableTableColumns);
        createViewStringBuilder.AppendFormat (
            "  SELECT {0}\r\n"
            + "    FROM [{1}].[{2}]",
            GetColumnList (unionedColumns),
            tableDefinition.TableName.SchemaName ?? DefaultSchema,
            tableDefinition.TableName.EntityName);

        numberOfSelects++;
      }
      if (numberOfSelects == 1)
        createViewStringBuilder.Append ("\r\n  WITH CHECK OPTION");
      statements.AddElement (new ScriptStatement (createViewStringBuilder.ToString()));

      statements.AddElement (CreateBatchDelimiterStatement());
      return statements;
    }
  }
}