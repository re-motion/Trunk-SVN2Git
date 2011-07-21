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
  /// The <see cref="SelectDbCommandBuilder"/> builds a command that allows retrieving a set of records where a certain column matches 
  /// a given <see cref="ObjectID"/> value.
  /// </summary>
  public class SelectDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _table;
    private readonly ISelectedColumnsSpecification _selectedColumns;
    private readonly ComparedColumnsSpecification _comparedColumnsSpecification;

    public SelectDbCommandBuilder (
        TableDefinition table,
        ISelectedColumnsSpecification selectedColumns,
        ComparedColumnsSpecification comparedColumnsSpecification,
        ISqlDialect sqlDialect,
        IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumnsSpecification", comparedColumnsSpecification);

      if(comparedColumnsSpecification.ComparedColumnValues.Count!=1) //TODO RM-4182: or add parameter for each column value?
        throw new ArgumentException ("Invalid compared columns count.", "comparedColumnsSpecification");

      _table = table;
      _selectedColumns = selectedColumns;
      _comparedColumnsSpecification = comparedColumnsSpecification;

      
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      var command = commandExecutionContext.CreateDbCommand();

      var statement = new StringBuilder();
      AppendSelectClause (statement, _selectedColumns);
      AppendFromClause (statement, _table);

      var parameter = AddCommandParameter (command, _table.ObjectIDColumn.Name, _comparedColumnsSpecification.ComparedColumnValues[0].Value);
      AppendComparingWhereClause (statement, _table.ObjectIDColumn, parameter);
      statement.Append (SqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString();

      return command;
    }
  }
}