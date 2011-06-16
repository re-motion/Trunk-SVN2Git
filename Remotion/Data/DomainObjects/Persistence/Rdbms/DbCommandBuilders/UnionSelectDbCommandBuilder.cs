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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public class UnionSelectDbCommandBuilder : DbCommandBuilder
  {
    // types

    // static members and constants

    public static UnionSelectDbCommandBuilder CreateForRelatedIDLookup (
        IStorageNameProvider storageNameProvider,
        ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID,
        ISqlDialect sqlDialect,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        ValueConverter valueConverter)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("valueConverter", valueConverter);

      return new UnionSelectDbCommandBuilder (
          storageNameProvider,
          classDefinition,
          propertyDefinition,
          relatedID,
          sqlDialect,
          rdbmsProviderDefinition,
          valueConverter);
    }

    // member fields

    private readonly ClassDefinition _classDefinition;
    private readonly PropertyDefinition _propertyDefinition;
    private readonly ObjectID _relatedID;
    private readonly IStorageNameProvider _storageNameProvider;

    // construction and disposing

    private UnionSelectDbCommandBuilder (
        IStorageNameProvider storageNameProvider,
        ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID,
        ISqlDialect sqlDialect,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        ValueConverter valueConverter)
        : base (sqlDialect, rdbmsProviderDefinition, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      _storageNameProvider = storageNameProvider;
      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedID = relatedID;
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public override IDbCommand Create (IDbCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      string[] allConcreteEntityNames = _classDefinition.GetAllConcreteEntityNames();
      if (allConcreteEntityNames.Length == 0)
        return null;

      IDbCommand command = commandFactory.CreateDbCommand();
      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);
      whereClauseBuilder.Add (_propertyDefinition.StoragePropertyDefinition.Name, _relatedID);

      var oppositeRelationEndPointDefinition =
          (VirtualRelationEndPointDefinition) _classDefinition.GetMandatoryOppositeEndPointDefinition (_propertyDefinition.PropertyName);

      string columnsFromSortExpression = GetColumnsFromSortExpression (oppositeRelationEndPointDefinition.GetSortExpression());

      var commandTextStringBuilder = new StringBuilder();
      string selectTemplate = "SELECT {0}, {1}{2} FROM {3} WHERE {4}";
      foreach (string entityName in allConcreteEntityNames)
      {
        if (commandTextStringBuilder.Length > 0)
          commandTextStringBuilder.Append ("\nUNION ALL ");

        commandTextStringBuilder.AppendFormat (
            selectTemplate,
            SqlDialect.DelimitIdentifier (StorageNameProvider.IDColumnName),
            SqlDialect.DelimitIdentifier (StorageNameProvider.ClassIDColumnName),
            columnsFromSortExpression,
            SqlDialect.DelimitIdentifier (entityName),
            whereClauseBuilder);
      }

      commandTextStringBuilder.Append (GetOrderClause (oppositeRelationEndPointDefinition.GetSortExpression()));
      commandTextStringBuilder.Append (";");

      command.CommandText = commandTextStringBuilder.ToString();

      return command;
    }

    private string GetColumnsFromSortExpression (SortExpressionDefinition sortExpression)
    {
      if (sortExpression == null)
        return string.Empty;

      var generator = new SortExpressionSqlGenerator (SqlDialect);
      return ", " + generator.GenerateColumnListString (sortExpression);
    }
  }
}