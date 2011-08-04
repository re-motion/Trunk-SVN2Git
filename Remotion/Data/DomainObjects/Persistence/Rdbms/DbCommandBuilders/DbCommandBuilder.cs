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
using System.Data;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public abstract class DbCommandBuilder : IDbCommandBuilder
  {
    private readonly ISqlDialect _sqlDialect;
    private readonly IValueConverter _valueConverter;

    protected DbCommandBuilder (ISqlDialect sqlDialect, IValueConverter valueConverter)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      ArgumentUtility.CheckNotNull ("valueConverter", valueConverter);

      _sqlDialect = sqlDialect;
      _valueConverter = valueConverter;
    }

    public abstract IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext);

    public ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    public IValueConverter ValueConverter
    {
      get { return _valueConverter; }
    }

    public IDataParameter AddCommandParameter (IDbCommand command, string parameterName, PropertyValue propertyValue)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      var value = propertyValue.GetValueWithoutEvents (ValueAccess.Current);
      IDataParameter commandParameter = AddCommandParameter (command, parameterName, value);

      // The default DbType for null values is String, which is alright for most data types, but not for Binary. Therefore, explicitly set the
      // DbType to binary for byte[]s.
      if (ReflectionUtility.IsBinaryPropertyValueType (propertyValue.Definition.PropertyType))
      {
        Assertion.IsTrue (commandParameter.DbType == DbType.Binary || value == null);
        commandParameter.DbType = DbType.Binary;
      }

      return commandParameter;
    }

    /// <remarks>
    /// This method cannot be used for binary (BLOB) <paramref name="parameterValue"/>. Use the overload with a <see cref="PropertyValue"/> instead.
    /// </remarks>
    public IDataParameter AddCommandParameter (IDbCommand command, string parameterName, object parameterValue)
    {
      // Note: UpdateCommandBuilder implicitly uses this method through WhereClauseBuilder.Add for Timestamp values.
      // Although Timestamp values are represented as byte arrays in ADO.NET with SQL Server they are no BLOB data type.
      // Therefore this usage is still valid.

      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);

      IDataParameter commandParameter = command.CreateParameter();
      commandParameter.ParameterName = SqlDialect.GetParameterName (parameterName);
      commandParameter.Value = ValueConverter.GetDBValue (parameterValue);

      command.Parameters.Add (commandParameter);
      return commandParameter;
    }

    protected string GetOrderClause (SortExpressionDefinition sortExpression)
    {
      if (sortExpression == null)
        return string.Empty;

      var generator = new SortExpressionSqlGenerator (SqlDialect);
      var orderByClause = generator.GenerateOrderByClauseString (sortExpression);
      return " " + orderByClause;
    }

    protected void AppendSelectClause (StringBuilder statement, ISelectedColumnsSpecification selectedColumns)
    {
      statement.Append ("SELECT ");
      selectedColumns.AppendProjection (statement, SqlDialect);
    }

    protected void AppendFromClause (StringBuilder statement, TableDefinition tableDefinition)
    {
      statement.Append (" FROM ");
      AppendTableName (statement, tableDefinition);
    }

    protected void AppendTableName (StringBuilder statement, TableDefinition tableDefinition)
    {
      if (tableDefinition.TableName.SchemaName != null)
      {
        statement.Append (SqlDialect.DelimitIdentifier (tableDefinition.TableName.SchemaName));
        statement.Append (".");
      }
      statement.Append (SqlDialect.DelimitIdentifier (tableDefinition.TableName.EntityName));
    }

    protected void AppendComparingWhereClause (StringBuilder statement, ColumnDefinition comparedColumn, IDataParameter expectedValue)
    {
      statement.Append (" WHERE ");
      statement.Append (SqlDialect.DelimitIdentifier (comparedColumn.Name));
      statement.Append (" = ");
      statement.Append (expectedValue.ParameterName);
    }

    protected void AppendWhereClause (StringBuilder statement, IComparedColumnsSpecification comparedColumns, IDbCommand command)
    {
      AppendWhereClause (statement, comparedColumns, command, null);
    }

    protected void AppendWhereClause (
        StringBuilder statement,
        IComparedColumnsSpecification comparedColumns,
        IDbCommand command,
        IDictionary<ColumnValue, IDbDataParameter> parameterCache)
    {
      statement.Append (" WHERE ");
      comparedColumns.AppendComparisons (statement, command, SqlDialect, parameterCache);
    }

    protected void AppendOrderByClause (StringBuilder statement, IOrderedColumnsSpecification orderedColumnsSpecification)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("orderedColumnsSpecification", orderedColumnsSpecification);

      if (!orderedColumnsSpecification.IsEmpty)
      {
        statement.Append (" ORDER BY ");
        orderedColumnsSpecification.AppendOrderings (statement, SqlDialect);
      }
    }
  }
}