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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class SingleIDLookupCommandBuilder: CommandBuilder
  {
    // types

    // static members and constants

    public static ICommandBuilder CreateForIDLookup (RdbmsProvider provider, string selectColumns, string entityName, ObjectID[] ids)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("ids", ids);

      if (ids.Length == 1)
        return new SingleIDLookupCommandBuilder (provider, selectColumns, entityName, "ID", ids[0], null);
      else
        return new MultiIDLookupCommandBuilder (provider, selectColumns, entityName, ids);
    }

    // member fields

    private readonly string _selectColumns;
    private readonly string _entityName;
    private readonly string _whereClauseColumnName;
    private readonly ObjectID _whereClauseID;
    private readonly string _orderExpression;

    // construction and disposing

    public SingleIDLookupCommandBuilder (
        RdbmsProvider provider, 
        string selectColumns, 
        string entityName, 
        string whereClauseColumnName, 
        ObjectID whereClauseID, 
        string orderExpression)
      : base (provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNullOrEmpty ("whereClauseColumnName", whereClauseColumnName);
      ArgumentUtility.CheckNotNull ("whereClauseID", whereClauseID);

      _selectColumns = selectColumns;
      _entityName = entityName;
      _whereClauseColumnName = whereClauseColumnName;
      _whereClauseID = whereClauseID;
      _orderExpression = orderExpression;
    }

    // methods and properties

    public string SelectColumns
    {
      get { return _selectColumns; }
    }

    public string EntityName
    {
      get { return _entityName; }
    }

    public string WhereClauseColumnName
    {
      get { return _whereClauseColumnName; }
    }

    public ObjectID WhereClauseID
    {
      get { return _whereClauseID; }
    }

    public string OrderExpression
    {
      get { return _orderExpression; }
    }

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();
      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);

      whereClauseBuilder.Add (_whereClauseColumnName, GetObjectIDValueForParameter (_whereClauseID));

      // TODO in case of integer primary keys: 
      // If RdbmsProvider or one of its derived classes will support integer primary keys in addition to GUIDs,
      // the code below must be selectively actived to run only for integer primary keys.
      // Note: This behaviour is not desired in case of GUID primary keys, because two same foreign key GUIDs pointing 
      //       to different classIDs must be an error! In this case PersistenceManager.CheckClassIDForVirtualEndPoint raises an exception. 
      //if (_whereClauseValueIsRelatedID && _whereClauseID.ClassDefinition.IsPartOfInheritanceHierarchy && IsOfSameStorageProvider (_whereClauseID))
      //  whereClauseBuilder.Add (RdbmsProvider.GetClassIDColumnName (_whereClauseColumnName), _whereClauseID.ClassID);

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
