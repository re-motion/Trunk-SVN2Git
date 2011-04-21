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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class SqlScriptIndexBuilder : IndexScriptBuilderBase, ISqlIndexDefinitionVisitor
  {
    public SqlScriptIndexBuilder ()
        : base (SqlServer.SqlDialect.Instance)
    {
    }

    public override string IndexStatementSeparator
    {
      get { return "GO\r\n\r\n"; }
    }

    void ISqlIndexDefinitionVisitor.VisitIndexDefinition (SqlIndexDefinition sqlIndexDefinition)
    {
      ArgumentUtility.CheckNotNull ("sqlIndexDefinition", sqlIndexDefinition);

      AppendSeparator();
      AddToCreateIndexScript (sqlIndexDefinition);
      AddToDropIndexScript (sqlIndexDefinition);
    }

    void ISqlIndexDefinitionVisitor.VisitPrimaryXmlIndexDefinition (SqlPrimaryXmlIndexDefinition primaryXmlIndexDefinition)
    {
      AppendSeparator();
      AddToCreateIndexScript (primaryXmlIndexDefinition);
      AddToDropIndexScript (primaryXmlIndexDefinition);
    }

    void ISqlIndexDefinitionVisitor.VisitSecondaryXmlIndexDefinition (SqlSecondaryXmlIndexDefinition secondaryXmlIndexDefinition)
    {
      AppendSeparator();
      AddToCreateIndexScript (secondaryXmlIndexDefinition);
      AddToDropIndexScript (secondaryXmlIndexDefinition);
    }

    private void AddToCreateIndexScript (SqlIndexDefinition sqlIndexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE {0}{1} INDEX [{2}]\r\n"
          + "  ON [{3}].[{4}] ({5})\r\n{6}{7}",
          sqlIndexDefinition.IsUnique.HasValue && sqlIndexDefinition.IsUnique.Value ? "UNIQUE " : string.Empty,
          sqlIndexDefinition.IsClustered.HasValue && sqlIndexDefinition.IsClustered.Value ? "CLUSTERED" : "NONCLUSTERED",
          sqlIndexDefinition.IndexName,
          sqlIndexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          sqlIndexDefinition.ObjectName.EntityName,
          GetColumnList (sqlIndexDefinition.Columns.Cast<IColumnDefinition>(), false),
          sqlIndexDefinition.IncludedColumns != null ? "  INCLUDE (" + GetColumnList (sqlIndexDefinition.IncludedColumns, false) + ")\r\n" : string.Empty,
          GetCreateIndexOptions (GetCreateIndexOptionItems (sqlIndexDefinition)));
    }

    private void AddToCreateIndexScript (SqlPrimaryXmlIndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE PRIMARY XML INDEX [{0}]\r\n"
          + "  ON [{1}].[{2}] ([{3}])\r\n{4}",
          indexDefinition.IndexName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name,
          GetCreateIndexOptions (GetCreateIndexOptionItemsBase (indexDefinition)));
    }

    private void AddToCreateIndexScript (SqlSecondaryXmlIndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendFormat (
          "CREATE XML INDEX [{0}]\r\n"
          + "  ON [{1}].[{2}] ([{3}])\r\n"
          + "  USING XML INDEX [{4}]\r\n"
          + "  FOR {5}\r\n{6}",
          indexDefinition.IndexName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.ObjectName.EntityName,
          indexDefinition.XmlColumn.Name,
          indexDefinition.PrimaryIndexName,
          indexDefinition.Kind,
          GetCreateIndexOptions (GetCreateIndexOptionItemsBase (indexDefinition)));
    }

    private void AddToDropIndexScript (IIndexDefinition indexDefinition)
    {
      DropIndexStringBuilder.AppendFormat (
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
          + "WHERE so.[name] = '{0}' and schema_name (so.schema_id)='{1}' and si.[name] = '{2}')\r\n"
          + "  DROP INDEX [{2}] ON [{1}].[{0}]\r\n",
          indexDefinition.ObjectName.EntityName,
          indexDefinition.ObjectName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
          indexDefinition.IndexName);
    }

    private string GetCreateIndexOptions (IEnumerable<string> optionItems)
    {
      return optionItems.Any () ? "  WITH (" + SeparatedStringBuilder.Build (", ", optionItems) + ")\r\n" : string.Empty;
    }

    private IEnumerable<string> GetCreateIndexOptionItemsBase (SqlIndexDefinitionBase indexDefinition)
    {
      if (indexDefinition.PadIndex.HasValue)
        yield return string.Format ("PAD_INDEX = {0}", indexDefinition.PadIndex.Value ? "ON" : "OFF");
      if (indexDefinition.FillFactor.HasValue)
        yield return string.Format ("FILLFACTOR = {0}", indexDefinition.FillFactor.Value);
      if (indexDefinition.SortInDb.HasValue)
        yield return string.Format ("SORT_IN_TEMPDB = {0}", indexDefinition.SortInDb.Value ? "ON" : "OFF");
      if (indexDefinition.StatisticsNoReCompute.HasValue)
        yield return string.Format ("STATISTICS_NORECOMPUTE = {0}", indexDefinition.StatisticsNoReCompute.Value ? "ON" : "OFF");
      if (indexDefinition.DropExisiting.HasValue)
        yield return string.Format ("DROP_EXISTING = {0}", indexDefinition.DropExisiting.Value ? "ON" : "OFF");
      if (indexDefinition.AllowRowLocks.HasValue)
        yield return string.Format ("ALLOW_ROW_LOCKS = {0}", indexDefinition.AllowRowLocks.Value ? "ON" : "OFF");
      if (indexDefinition.AllowPageLocks.HasValue)
        yield return string.Format ("ALLOW_PAGE_LOCKS = {0}", indexDefinition.AllowPageLocks.Value ? "ON" : "OFF");
      if (indexDefinition.MaxDop.HasValue)
        yield return string.Format ("MAXDOP = {0}", indexDefinition.MaxDop.Value);
    }

    private IEnumerable<string> GetCreateIndexOptionItems (SqlIndexDefinition indexDefinition)
    {
      if (indexDefinition.IgnoreDupKey.HasValue)
        yield return string.Format ("IGNORE_DUP_KEY = {0}", indexDefinition.IgnoreDupKey.Value ? "ON" : "OFF");
      if (indexDefinition.Online.HasValue)
        yield return string.Format ("ONLINE = {0}", indexDefinition.Online.Value ? "ON" : "OFF");
      foreach (var optionItem in GetCreateIndexOptionItemsBase (indexDefinition))
        yield return optionItem;
    }
    
  }
}