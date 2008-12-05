// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

    public static SelectCommandBuilder CreateForIDLookup (RdbmsProvider provider, string selectColumns, string entityName, ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("id", id);

      return new SelectCommandBuilder (provider, selectColumns, entityName, "ID", new ObjectID[] {id}, false, null);
    }

    public static SelectCommandBuilder CreateForIDLookup (RdbmsProvider provider, string entityName, ObjectID[] ids)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("ids", ids);

      return new SelectCommandBuilder (provider, "*", entityName, "ID", ids, false, null);
    }

    public static SelectCommandBuilder CreateForRelatedIDLookup (
        RdbmsProvider provider,
        ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      VirtualRelationEndPointDefinition oppositeRelationEndPointDefinition =
          (VirtualRelationEndPointDefinition) classDefinition.GetMandatoryOppositeEndPointDefinition (propertyDefinition.PropertyName);

      return new SelectCommandBuilder (
          provider,
          "*",
          classDefinition.GetEntityName(),
          propertyDefinition.StorageSpecificName,
          new ObjectID[] {relatedID},
          true,
          oppositeRelationEndPointDefinition.SortExpression);
    }

    // member fields

    private string _selectColumns;
    private string _entityName;
    private string _whereClauseColumnName;
    private ObjectID[] _whereClauseIDs;
    private bool _whereClauseValueIsRelatedID;
    private string _orderExpression;

    // construction and disposing

    private SelectCommandBuilder (
        RdbmsProvider provider,
        string selectColumns,
        string entityName,
        string whereClauseColumnName,
        ObjectID[] whereClauseIDs,
        bool whereClauseValueIsRelatedID,
        string orderExpression)
        : base (provider)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNullOrEmpty ("whereClauseColumnName", whereClauseColumnName);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("whereClauseIDs", whereClauseIDs);

      _selectColumns = selectColumns;
      _entityName = entityName;
      _whereClauseColumnName = whereClauseColumnName;
      _whereClauseIDs = whereClauseIDs;
      _whereClauseValueIsRelatedID = whereClauseValueIsRelatedID;
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
          whereClauseBuilder.ToString(),
          GetOrderClause (_orderExpression),
          Provider.StatementDelimiter);

      return command;
    }

    private object[] GetValueArrayForParameter (ObjectID[] objectIDs)
    {
      object[] values = new object[objectIDs.Length];

      for (int i = 0; i < objectIDs.Length; i++)
        values[i] = GetObjectIDValueForParameter (objectIDs[i]);

      return values;
    }

    protected override void AppendColumn (string columnName, string parameterName)
    {
      throw new NotSupportedException ("'AppendColumn' is not supported by 'QueryCommandBuilder'.");
    }
  }
}
