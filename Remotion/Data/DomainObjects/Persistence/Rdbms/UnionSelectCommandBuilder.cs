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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class UnionSelectCommandBuilder : CommandBuilder
  {
    // types

    // static members and constants

  public static UnionSelectCommandBuilder CreateForRelatedIDLookup (
      RdbmsProvider provider, 
      ClassDefinition classDefinition, 
      PropertyDefinition propertyDefinition,
      ObjectID relatedID)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("relatedID", relatedID);

    return new UnionSelectCommandBuilder (provider, classDefinition, propertyDefinition, relatedID);
  }

    // member fields

    private readonly ClassDefinition _classDefinition;
    private readonly PropertyDefinition _propertyDefinition;
    private readonly ObjectID _relatedID;

    // construction and disposing

    private UnionSelectCommandBuilder (      
        RdbmsProvider provider, 
        ClassDefinition classDefinition, 
        PropertyDefinition propertyDefinition,
        ObjectID relatedID) : base (provider)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedID = relatedID;
    }

    // methods and properties

    public override IDbCommand Create ()
    {
      string[] allConcreteEntityNames = _classDefinition.GetAllConcreteEntityNames ();
      if (allConcreteEntityNames.Length == 0)
        return null;

      IDbCommand command = Provider.CreateDbCommand ();
      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);
      whereClauseBuilder.Add (_propertyDefinition.StorageSpecificName, GetObjectIDValueForParameter (_relatedID));

      var oppositeRelationEndPointDefinition =
          (VirtualRelationEndPointDefinition) _classDefinition.GetMandatoryOppositeEndPointDefinition (_propertyDefinition.PropertyName);

      string columnsFromSortExpression = GetColumnsFromSortExpression (oppositeRelationEndPointDefinition.SortExpression);

      var commandTextStringBuilder = new StringBuilder ();
      string selectTemplate = "SELECT {0}, {1}{2} FROM {3} WHERE {4}";
      foreach (string entityName in allConcreteEntityNames)
      {
        if (commandTextStringBuilder.Length > 0)
          commandTextStringBuilder.Append ("\nUNION ALL ");

        commandTextStringBuilder.AppendFormat (selectTemplate, 
                  Provider.DelimitIdentifier ("ID"),
                  Provider.DelimitIdentifier ("ClassID"),
                  columnsFromSortExpression, 
                  Provider.DelimitIdentifier (entityName),
                  whereClauseBuilder);
      }

      commandTextStringBuilder.Append (GetOrderClause (oppositeRelationEndPointDefinition.SortExpression));
      commandTextStringBuilder.Append (";");
      
      command.CommandText = commandTextStringBuilder.ToString ();

      return command;
    }

    private string GetColumnsFromSortExpression (string sortExpression)
    {
      if (string.IsNullOrEmpty (sortExpression))
        return string.Empty;

      return ", " + Provider.GetColumnsFromSortExpression (sortExpression);
    }
  }
}
