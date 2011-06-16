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
using System.Data;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public class TableRelationLookupSelectDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _table;
    private readonly ISelectedColumnsSpecification _selectedColumns;
    private readonly SimpleColumnDefinition _foreignKeyColumn;
    private readonly ObjectID _foreignKeyValue;
    private readonly IOrderedColumnsSpecification _orderedColumns;

    public TableRelationLookupSelectDbCommandBuilder (
        TableDefinition table,
        ISelectedColumnsSpecification selectedColumns,
        SimpleColumnDefinition foreignKeyColumn,
        ObjectID foreignKeyValue,
        IOrderedColumnsSpecification orderedColumns,
        ISqlDialect sqlDialect,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        ValueConverter valueConverter)
      : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("foreignKeyColumn", foreignKeyColumn);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      _table = table;
      _selectedColumns = selectedColumns;
      _foreignKeyColumn = foreignKeyColumn;
      _foreignKeyValue = foreignKeyValue;
      _orderedColumns = orderedColumns;
    }

    public override IDbCommand Create (IDbCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      // TODO in case of integer primary keys: 
      // If RdbmsProvider or one of its derived classes will support integer primary keys in addition to GUIDs,
      // the code below must be selectively actived to run only for integer primary keys.
      // Note: This behaviour is not desired in case of GUID primary keys, because two same foreign key GUIDs pointing 
      //       to different classIDs must be an error! In this case PersistenceManager.CheckClassIDForVirtualEndPoint raises an exception. 
      //if (_whereClauseValueIsRelatedID && _expectedValue.ClassDefinition.IsPartOfInheritanceHierarchy && IsOfSameStorageProvider (_expectedValue))
      //  whereClauseBuilder.Add (RdbmsProvider.GetRelationClassIDColumnName (_checkedColumnName), _expectedValue.ClassID);

      var command = commandFactory.CreateDbCommand ();

      var statement = new StringBuilder ();
      statement.Append ("SELECT");
      _selectedColumns.AppendProjection (statement, SqlDialect);
      statement.Append ("FROM ");
      if (_table.TableName.SchemaName != null)
      {
        statement.Append (SqlDialect.DelimitIdentifier (_table.TableName.SchemaName));
        statement.Append (".");
      }
      statement.Append (SqlDialect.DelimitIdentifier (_table.TableName.EntityName));
      statement.Append (" WHERE ");
      statement.Append (SqlDialect.DelimitIdentifier (_foreignKeyColumn.Name));
      statement.Append (" = ");
      statement.Append (SqlDialect.GetParameterName (_foreignKeyColumn.Name));
      _orderedColumns.AppendOrderByClause (statement, SqlDialect);

      AddCommandParameter (command, SqlDialect.GetParameterName (_foreignKeyColumn.Name), _foreignKeyValue);

      command.CommandText = statement.ToString ();

      return command;
    }
  }
}