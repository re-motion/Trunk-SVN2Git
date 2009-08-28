// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
  public class SelectCommandBuilder: CommandBuilder
  {
    // types

    // static members and constants

    public static SelectCommandBuilder CreateForIDLookup (RdbmsProvider provider, string selectColumns, string entityName, ObjectID[] ids)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("ids", ids);

      return new SelectCommandBuilder (provider, selectColumns, entityName, "ID", ids, null);
    }

    public static SelectCommandBuilder CreateForRelatedIDLookup (
        RdbmsProvider provider,
        string entityName,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      var oppositeRelationEndPointDefinition = (VirtualRelationEndPointDefinition) 
          propertyDefinition.ClassDefinition.GetMandatoryOppositeEndPointDefinition (propertyDefinition.PropertyName);

      return new SelectCommandBuilder (
          provider,
          "*",
          entityName,
          propertyDefinition.StorageSpecificName,
          new[] {relatedID},
          oppositeRelationEndPointDefinition.SortExpression);
    }

    // member fields

    private readonly string _selectColumns;
    private readonly string _entityName;
    private readonly string _whereClauseColumnName;
    private readonly ObjectID[] _whereClauseIDs;
    private readonly string _orderExpression;

    // construction and disposing

    private SelectCommandBuilder (
        RdbmsProvider provider, 
        string selectColumns, 
        string entityName, 
        string whereClauseColumnName, 
        ObjectID[] whereClauseIDs, 
        string orderExpression)
      : base (provider)
    {
      _selectColumns = selectColumns;
      _entityName = entityName;
      _whereClauseColumnName = whereClauseColumnName;
      _whereClauseIDs = whereClauseIDs;
      _orderExpression = orderExpression;
    }

    // methods and properties

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();
      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);

      if (_whereClauseIDs.Length == 1)
        whereClauseBuilder.Add (_whereClauseColumnName, GetObjectIDValueForParameter (_whereClauseIDs[0]));
      else
        whereClauseBuilder.SetInExpression (_whereClauseColumnName, GetValueArrayForParameter (_whereClauseIDs));

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

    private object[] GetValueArrayForParameter (ObjectID[] objectIDs)
    {
      var values = new object[objectIDs.Length];

      for (int i = 0; i < objectIDs.Length; i++)
        values[i] = GetObjectIDValueForParameter (objectIDs[i]);

      return values;
    }
  }
}
