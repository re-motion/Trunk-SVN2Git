/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Data;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public abstract class CommandBuilder
  {
    // types

    // static members and constants

    // member fields

    private RdbmsProvider _provider;

    // construction and disposing

    protected CommandBuilder (RdbmsProvider provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);

      if (!provider.IsConnected)
        throw new ArgumentException ("Provider must be connected first.", "provider");

      _provider = provider;
    }

    // abstract methods and properties

    public abstract IDbCommand Create();

    // methods and properties

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public IDataParameter AddCommandParameter (IDbCommand command, string parameterName, PropertyValue propertyValue)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      IDataParameter commandParameter = AddCommandParameter (command, parameterName, propertyValue.GetFieldValue (ValueAccess.Current));

      if (propertyValue.PropertyType == typeof (byte[]))
        commandParameter.DbType = DbType.Binary;

      return commandParameter;
    }

    /// <remarks>
    /// This method cannot be used for binary (BLOB) <paramref name="parameterValues"/>. Use the overload with a <see cref="Remotion.Data.DomainObjects.PropertyValue"/> instead.
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

      ValueConverter valueConverter = Provider.CreateValueConverter ();
      if (parameterValue != null && parameterValue.GetType() == typeof (ObjectID))
        commandParameter.Value = valueConverter.GetDBValue ((ObjectID) parameterValue, Provider.ID);
      else
        commandParameter.Value = valueConverter.GetDBValue (parameterValue);

      command.Parameters.Add (commandParameter);
      return commandParameter;
    }

    protected void AddObjectIDAndClassIDParameters (
        IDbCommand command,
        ClassDefinition classDefinition,
        PropertyValue propertyValue)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      ClassDefinition relatedClassDefinition = null;
      object relatedIDValue = null;
      if (propertyValue.GetFieldValue (ValueAccess.Current) != null)
      {
        ObjectID relatedID = (ObjectID) propertyValue.GetFieldValue (ValueAccess.Current);
        relatedClassDefinition = relatedID.ClassDefinition;
        relatedIDValue = GetObjectIDValueForParameter (relatedID);
      }
      else
      {
        relatedClassDefinition = classDefinition.GetOppositeClassDefinition (propertyValue.Name);
        relatedIDValue = null;
      }

      AddCommandParameter (command, propertyValue.Definition.StorageSpecificName, relatedIDValue);

      if (classDefinition.StorageProviderID == relatedClassDefinition.StorageProviderID)
        AddClassIDParameter (command, relatedClassDefinition, propertyValue);
    }

    protected void AddClassIDParameter (
        IDbCommand command,
        ClassDefinition relatedClassDefinition,
        PropertyValue propertyValue)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("relatedClassDefinition", relatedClassDefinition);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      if (relatedClassDefinition.IsPartOfInheritanceHierarchy)
      {
        string classIDColumnName = RdbmsProvider.GetClassIDColumnName (propertyValue.Definition.StorageSpecificName);
        AppendColumn (classIDColumnName, classIDColumnName);

        string classID = null;
        if (propertyValue.GetFieldValue (ValueAccess.Current) != null)
          classID = relatedClassDefinition.ID;

        AddCommandParameter (command, classIDColumnName, classID);
      }
    }

    protected object GetValueForParameter (object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      if (value.GetType() == typeof (ObjectID))
        return GetObjectIDValueForParameter ((ObjectID) value);
      else
        return value;
    }

    protected abstract void AppendColumn (string columnName, string parameterName);

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

      return id.StorageProviderID == _provider.ID;
    }

    protected string GetOrderClause (string orderExpression)
    {
      if (orderExpression != null && orderExpression != string.Empty)
        return " ORDER BY " + orderExpression;

      return string.Empty;
    }

    protected ArgumentException CreateArgumentException (string parameterName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), parameterName);
    }
  }
}
