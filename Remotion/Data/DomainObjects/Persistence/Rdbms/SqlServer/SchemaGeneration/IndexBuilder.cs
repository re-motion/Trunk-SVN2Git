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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class IndexBuilder : IndexBuilderBase, ISqlIndexDefinitionVisitor
  {
    public IndexBuilder (ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
    }

    public override string IndexStatementSeparator
    {
      get { return "GO\r\n\r\n"; }
    }

    public void VisitIndexDefinition (IndexDefinition indexDefinition)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      AppendSeparator();
      AddToCreateIndexScript (indexDefinition);
      AddToDropIndexScript (indexDefinition);
    }

    public void VisitPrimaryXmlIndexDefinition (PrimaryXmlIndexDefinition primaryXmlIndexDefinition)
    {
      AppendSeparator ();
      AddToCreateIndexScript (primaryXmlIndexDefinition);
      AddToDropIndexScript (primaryXmlIndexDefinition);
    }

    public void VisitSecondaryXmlIndexDefinition (SecondaryXmlIndexDefinition secondaryXmlIndexDefinition)
    {
      AppendSeparator ();
      AddToCreateIndexScript (secondaryXmlIndexDefinition);
      AddToDropIndexScript (secondaryXmlIndexDefinition);
    }

    private void AddToCreateIndexScript (IndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE {0}{1} INDEX [{2}].[{3}]\r\n"
          + "  ON [{4}].[{5}] ({6})\r\n{7}"
          + "  WITH IGNORE_DUP_KEY = {8}, ONLINE = {9}\r\n",
          indexDefinition.IsUnique ? "UNIQUE " : string.Empty,
          indexDefinition.IsClustered ? "CLUSTERED" : "NONCLUSTERED",
          indexDefinition.IndexName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.IndexName.EntityName,
          indexDefinition.ObjectName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          GetColumnList (indexDefinition.Columns, false),
          indexDefinition.IncludedColumns!=null ? "  INCLUDE ("+GetColumnList (indexDefinition.IncludedColumns, false)+")\r\n" : string.Empty,
          indexDefinition.IgnoreDupKey ? "ON" : "OFF",
          indexDefinition.Online ? "ON" : "OFF");
    }

    private void AddToCreateIndexScript (PrimaryXmlIndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE PRIMARY XML INDEX [{0}].[{1}]\r\n"
          + "  ON [{2}].[{3}] ([{4}])\r\n",
          indexDefinition.IndexName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.IndexName.EntityName,
          indexDefinition.ObjectName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name);
    }

    private void AddToCreateIndexScript (SecondaryXmlIndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE XML INDEX [{0}].[{1}]\r\n"
          + "  ON [{2}].[{3}] ([{4}])\r\n"
          + "  USING XML INDEX [{5}].[{6}]\r\n"
          + "  FOR {7}\r\n",
          indexDefinition.IndexName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.IndexName.EntityName,
          indexDefinition.ObjectName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name,
          indexDefinition.PrimaryIndexName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.PrimaryIndexName.EntityName,
          indexDefinition.Kind);
    }

    private void AddToDropIndexScript (IIndexDefinition indexDefinition)
    {
      DropIndexStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
          +"WHERE so.[name] = '{0}' and schema_name (so.schema_id)='{1}' and si.[name] = '{2}')\r\n"
          + "  DROP INDEX [{3}].[{2}]\r\n",
          indexDefinition.ObjectName.EntityName,
          indexDefinition.ObjectName.SchemaName ?? ScriptBuilder.DefaultSchema,
          indexDefinition.IndexName.EntityName,
          indexDefinition.IndexName.SchemaName ?? ScriptBuilder.DefaultSchema);
    }
  }
}