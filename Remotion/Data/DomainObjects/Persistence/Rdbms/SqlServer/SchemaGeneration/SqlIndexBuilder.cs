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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class SqlIndexBuilder : IndexBuilderBase, ISqlIndexDefinitionVisitor
  {
    public SqlIndexBuilder ()
        : base(SqlServer.SqlDialect.Instance)
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
          "CREATE {0}{1} INDEX [{2}]\r\n"
          + "  ON [{3}].[{4}] ({5})\r\n{6}{7}",
          indexDefinition.IsUnique ? "UNIQUE " : string.Empty,
          indexDefinition.IsClustered ? "CLUSTERED" : "NONCLUSTERED",
          indexDefinition.IndexName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          GetColumnList (indexDefinition.Columns, false),
          indexDefinition.IncludedColumns!=null ? "  INCLUDE ("+GetColumnList (indexDefinition.IncludedColumns, false)+")\r\n" : string.Empty,
          GetCreateIndexOptions (indexDefinition));
    }

    private void AddToCreateIndexScript (PrimaryXmlIndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE PRIMARY XML INDEX [{0}]\r\n"
          + "  ON [{1}].[{2}] ([{3}])\r\n",
          indexDefinition.IndexName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name);
    }

    private void AddToCreateIndexScript (SecondaryXmlIndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE XML INDEX [{0}]\r\n"
          + "  ON [{1}].[{2}] ([{3}])\r\n"
          + "  USING XML INDEX [{4}]\r\n"
          + "  FOR {5}\r\n",
          indexDefinition.IndexName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name,
          indexDefinition.PrimaryIndexName,
          indexDefinition.Kind);
    }

    private void AddToDropIndexScript (IIndexDefinition indexDefinition)
    {
      DropIndexStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
          +"WHERE so.[name] = '{0}' and schema_name (so.schema_id)='{1}' and si.[name] = '{2}')\r\n"
          + "  DROP INDEX [{2}] ON [{1}].[{0}]\r\n",
          indexDefinition.ObjectName.EntityName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.IndexName);
    }

    private string GetCreateIndexOptions (IndexDefinition indexDefinition)
    {
      var options = new StringBuilder (string.Empty);
      if (indexDefinition.IgnoreDupKey || indexDefinition.Online)
      {
        if (indexDefinition.IgnoreDupKey)
          AddOption ("IGNORE_DUP_KEY", options);
        if (indexDefinition.Online)
          AddOption ("ONLINE", options);
        options.Insert(0, "  WITH ");
        options.Append ("\r\n");
      }
      return options.ToString ();
    }

    private void AddOption (string option, StringBuilder options)
    {
      if (options.Length > 0)
        options.Append (", ");
      options.Append (option);
    }
  }
}