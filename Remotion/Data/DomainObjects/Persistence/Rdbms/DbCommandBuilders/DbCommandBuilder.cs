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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public abstract class DbCommandBuilder : IDbCommandBuilder
  {
    private readonly RdbmsProvider _provider;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly ISqlDialect _sqlDialect;
    private readonly IDbCommandFactory _commandFactory;

    protected DbCommandBuilder (
        RdbmsProvider provider, IStorageNameProvider storageNameProvider, ISqlDialect sqlDialect, IDbCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      _provider = provider;
      _storageNameProvider = storageNameProvider;
      _sqlDialect = sqlDialect;
      _commandFactory = commandFactory;
    }

    public abstract IDbCommand Create ();

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    public IDbCommandFactory CommandFactory
    {
      get { return _commandFactory; }
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
      commandParameter.ParameterName = Provider.GetParameterName (parameterName);

      ValueConverter valueConverter = Provider.CreateValueConverter();
      if (parameterValue is ObjectID)
        commandParameter.Value = valueConverter.GetDBValue ((ObjectID) parameterValue, Provider.StorageProviderDefinition.Name);
      else
        commandParameter.Value = valueConverter.GetDBValue (parameterValue);

      command.Parameters.Add (commandParameter);
      return commandParameter;
    }

    protected object GetValueForParameter (object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      if (value is ObjectID)
        return GetObjectIDValueForParameter ((ObjectID) value);
      else
        return value;
    }

    protected object GetObjectIDValueForParameter (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      if (IsOfSameStorageProvider (id))
        return id.Value;
      else
        return id.ToString();
    }

    protected bool IsOfSameStorageProvider (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      return id.StorageProviderDefinition == _provider.StorageProviderDefinition;
    }

    protected string GetOrderClause (SortExpressionDefinition sortExpression)
    {
      if (sortExpression == null)
        return string.Empty;

      var generator = Provider.GetSortExpressionSqlGenerator();
      var orderByClause = generator.GenerateOrderByClauseString (sortExpression);
      return " " + orderByClause;
    }

    protected ArgumentException CreateArgumentException (string parameterName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), parameterName);
    }
  }
}