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
  /// <summary>
  /// The <see cref="SingleIDLookupSelectDbCommandBuilder"/> builds a command that allows retrieving a set of records where a certain column matches 
  /// a given <see cref="ObjectID"/> value.
  /// </summary>
  public class SingleIDLookupSelectDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _table;
    private readonly ISelectedColumnsSpecification _selectedColumns;
    private readonly ObjectID _objectID;

    public SingleIDLookupSelectDbCommandBuilder (
        TableDefinition table, ISelectedColumnsSpecification selectedColumns, ObjectID objectID, ISqlDialect sqlDialect, IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _table = table;
      _selectedColumns = selectedColumns;
      _objectID = objectID;
    }

    public override IDbCommand Create (IDbCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      var command = commandFactory.CreateDbCommand();

      var statement = new StringBuilder();
      AppendSelectClause (statement, _selectedColumns);
      AppendFromClause(statement, _table);

      var parameter = AddCommandParameter (command, _table.ObjectIDColumn.Name, _objectID);
      AppendComparingWhereClause (statement, _table.ObjectIDColumn, parameter);
      
      command.CommandText = statement.ToString();

      return command;
    }
  }
}