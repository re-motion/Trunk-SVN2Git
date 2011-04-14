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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// Generates SQL Server-specific views for the persistence model.
  /// </summary>
  public class ViewBuilder : ViewBuilderBase
  {
    public ViewBuilder () : base(SqlDialect.Instance)
    {
    }

    public override string CreateViewSeparator
    {
      get { return "GO\r\n\r\n"; }
    }

    public override void AddFilterViewToCreateViewScript (FilterViewDefinition filterViewDefinition, StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT {2}\r\n"
          + "    FROM [{3}].[{4}]\r\n"
          + "    WHERE [ClassID] IN ({5})\r\n"
          + "  WITH CHECK OPTION\r\n",
          filterViewDefinition.ViewName.SchemaName ?? ScriptBuilder.DefaultSchema,
          filterViewDefinition.ViewName.EntityName,
          GetColumnList (filterViewDefinition.GetColumns(), false),
          filterViewDefinition.GetBaseTable().TableName.SchemaName ?? ScriptBuilder.DefaultSchema,
          filterViewDefinition.GetBaseTable().TableName.EntityName,
          GetClassIDList (filterViewDefinition.ClassIDs));
    }

    public override void AddTableViewToCreateViewScript (
        TableDefinition tableDefinition,
        StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT {2}\r\n"
          + "    FROM [{3}].[{4}]\r\n"
          + "  WITH CHECK OPTION\r\n",
          tableDefinition.ViewName.SchemaName ?? ScriptBuilder.DefaultSchema,
          tableDefinition.ViewName.EntityName,
          GetColumnList (tableDefinition.GetColumns (), false),
          tableDefinition.TableName.SchemaName ?? ScriptBuilder.DefaultSchema,
          tableDefinition.TableName.EntityName);
    }

    public override void AddUnionViewToCreateViewScript (
        UnionViewDefinition unionViewDefinition,
        StringBuilder createViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);
      ArgumentUtility.CheckNotNull ("createViewStringBuilder", createViewStringBuilder);

      createViewStringBuilder.AppendFormat (
          "CREATE VIEW [{0}].[{1}] ({2})\r\n"
          + "  WITH SCHEMABINDING AS\r\n",
          unionViewDefinition.ViewName.SchemaName ?? ScriptBuilder.DefaultSchema,
          unionViewDefinition.ViewName.EntityName,
          GetColumnList (unionViewDefinition.GetColumns (), false));

      int numberOfSelects = 0;
      foreach (var tableDefinition in unionViewDefinition.GetAllTables())
      {
        if (numberOfSelects > 0)
          createViewStringBuilder.AppendFormat ("  UNION ALL\r\n");

        var availableTableColumns = tableDefinition.GetColumns();
        var unionedColumns = unionViewDefinition.CreateFullColumnList (availableTableColumns);
        createViewStringBuilder.AppendFormat (
            "  SELECT {0}\r\n"
            + "    FROM [{1}].[{2}]\r\n",
            GetColumnList (unionedColumns, true),
            tableDefinition.TableName.SchemaName ?? ScriptBuilder.DefaultSchema,
            tableDefinition.TableName.EntityName);

        numberOfSelects++;
      }
      if (numberOfSelects == 1)
        createViewStringBuilder.Append ("  WITH CHECK OPTION\r\n");
    }

    public override void AddToDropViewScript (IEntityDefinition entityDefinition, StringBuilder dropViewStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("dropViewStringBuilder", dropViewStringBuilder);

      dropViewStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\r\n"
          + "  DROP VIEW [{1}].[{0}]\r\n",
          entityDefinition.ViewName.EntityName,
          entityDefinition.ViewName.SchemaName ?? ScriptBuilder.DefaultSchema);
    }
    
  }
}
