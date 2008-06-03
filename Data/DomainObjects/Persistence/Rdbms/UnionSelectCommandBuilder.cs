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

    private ClassDefinition _classDefinition;
    private PropertyDefinition _propertyDefinition;
    private ObjectID _relatedID;

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

      VirtualRelationEndPointDefinition oppositeRelationEndPointDefinition =
          (VirtualRelationEndPointDefinition) _classDefinition.GetMandatoryOppositeEndPointDefinition (_propertyDefinition.PropertyName);

      string columnsFromSortExpression = GetColumnsFromSortExpression (oppositeRelationEndPointDefinition.SortExpression);

      StringBuilder commandTextStringBuilder = new StringBuilder ();
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
                  whereClauseBuilder.ToString ());
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

    protected override void AppendColumn (string columnName, string parameterName)
    {
      throw new NotSupportedException ("'AppendColumn' is not supported by 'QueryCommandBuilder'.");
    }
  }
}
