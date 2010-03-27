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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Builds a command that allows retrieving a set of records where a certain column matches a given <see cref="ObjectID"/> value.
  /// </summary>
  public class SingleIDLookupCommandBuilder: CommandBuilder
  {
    private readonly string _selectColumns;
    private readonly string _entityName;
    private readonly string _checkedColumnName;
    private readonly ObjectID _expectedValue;
    private readonly string _orderExpression;

    public SingleIDLookupCommandBuilder (
        RdbmsProvider provider, 
        string selectColumns, 
        string entityName, 
        string checkedColumnName, 
        ObjectID expectedValue, 
        string orderExpression)
      : base (provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNullOrEmpty ("checkedColumnName", checkedColumnName);
      ArgumentUtility.CheckNotNull ("expectedValue", expectedValue);

      _selectColumns = selectColumns;
      _entityName = entityName;
      _checkedColumnName = checkedColumnName;
      _expectedValue = expectedValue;
      _orderExpression = orderExpression;
    }

    public string SelectColumns
    {
      get { return _selectColumns; }
    }

    public string EntityName
    {
      get { return _entityName; }
    }

    public string CheckedColumnName
    {
      get { return _checkedColumnName; }
    }

    public ObjectID ExpectedValue
    {
      get { return _expectedValue; }
    }

    public string OrderExpression
    {
      get { return _orderExpression; }
    }

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();
      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);

      whereClauseBuilder.Add (_checkedColumnName, GetObjectIDValueForParameter (_expectedValue));

      // TODO in case of integer primary keys: 
      // If RdbmsProvider or one of its derived classes will support integer primary keys in addition to GUIDs,
      // the code below must be selectively actived to run only for integer primary keys.
      // Note: This behaviour is not desired in case of GUID primary keys, because two same foreign key GUIDs pointing 
      //       to different classIDs must be an error! In this case PersistenceManager.CheckClassIDForVirtualEndPoint raises an exception. 
      //if (_whereClauseValueIsRelatedID && _expectedValue.ClassDefinition.IsPartOfInheritanceHierarchy && IsOfSameStorageProvider (_expectedValue))
      //  whereClauseBuilder.Add (RdbmsProvider.GetClassIDColumnName (_checkedColumnName), _expectedValue.ClassID);

      command.CommandText = string.Format (
          "SELECT {0} FROM {1} WHERE {2}{3}{4}",
          _selectColumns,
          Provider.DelimitIdentifier (_entityName),
          whereClauseBuilder,
          GetOrderClause (_orderExpression),
          Provider.StatementDelimiter);

      return command;
    }
  }
}
