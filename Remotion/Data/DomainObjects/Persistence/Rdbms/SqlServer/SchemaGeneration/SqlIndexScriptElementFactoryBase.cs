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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// <see cref="SqlIndexScriptElementFactoryBase{T}"/> represents the base-class for all factory classes that are responsible to create new script 
  /// elements for creating indexes in a relational database.
  /// </summary>
  public abstract class SqlIndexScriptElementFactoryBase<T> : ISqlIndexDefinitionScriptElementFactory<T> where T:SqlIndexDefinitionBase
  {
    public abstract IScriptElement GetCreateElement (T indexDefinition);

    public virtual IScriptElement GetDropElement (T indexDefinition)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      return new ScriptStatement (
          string.Format (
              "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
              + "WHERE so.[name] = '{0}' and schema_name (so.schema_id)='{1}' and si.[name] = '{2}')\r\n"
              + "  DROP INDEX [{2}] ON [{1}].[{0}]",
              indexDefinition.ObjectName.EntityName,
              indexDefinition.ObjectName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
              indexDefinition.IndexName));
    
    }

    protected string GetColumnList (IEnumerable<IColumnDefinition> columnDefinitions)
    {
      return NameListColumnDefinitionVisitor.GetNameList (columnDefinitions, false, SqlDialect.Instance);
    }

    protected string GetIndexedColumnNames (IEnumerable<SqlIndexedColumnDefinition> indexedColumnDefinitions)
    {
      return SeparatedStringBuilder.Build (
          ", ",
          indexedColumnDefinitions.Select (
              cd => "["+cd.Columnn.Name+"]"+ (cd.IndexOrder.HasValue ? " " + cd.IndexOrder.ToString ().ToUpper () : string.Empty)));
    }

    protected virtual string GetCreateIndexOptions (IEnumerable<string> optionItems)
    {
      return optionItems.Any () ? "\r\n  WITH (" + SeparatedStringBuilder.Build (", ", optionItems) + ")" : string.Empty;
    }

    protected virtual IEnumerable<string> GetCreateIndexOptionItems (SqlIndexDefinitionBase indexDefinition)
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
  }
}