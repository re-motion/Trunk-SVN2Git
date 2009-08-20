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
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class UpdateCommandBuilder : CommandBuilder
  {
    // types

    // static members and constants

    // member fields

    private readonly DataContainer _dataContainer;

    // construction and disposing

    public UpdateCommandBuilder (RdbmsProvider provider, DataContainer dataContainer)
        : base (provider)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State == StateType.Unchanged)
        throw CreateArgumentException ("dataContainer", "State of provided DataContainer must not be 'Unchanged'.");

      _dataContainer = dataContainer;
    }

    // methods and properties

    public override IDbCommand Create ()
    {
      IDbCommand command = Provider.CreateDbCommand();
      var updateSetBuilder = new StringBuilder();

      foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
      {
        if (propertyValue.Definition.StorageClass == StorageClass.Persistent && MustBeUpdated (propertyValue))
          AddPropertyValue (updateSetBuilder, command, propertyValue);
      }

      if (command.Parameters.Count == 0 && !_dataContainer.HasBeenMarkedChanged)
      {
        command.Dispose();
        return null;
      }

      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);
      whereClauseBuilder.Add ("ID", _dataContainer.ID.Value);

      if (_dataContainer.State != StateType.New)
        whereClauseBuilder.Add ("Timestamp", _dataContainer.Timestamp);

      if (updateSetBuilder.Length == 0)
      {
        // SET [ClassID] = [ClassID] => dummy set if no other property is set
        // This occurs whenever the Timestamp should be incremented even though no property was changed. Used, e.g., via DomainObject.MarkAsChanged.
        updateSetBuilder.AppendFormat (
            "{0} = {1}",
            Provider.DelimitIdentifier ("ClassID"),
            Provider.DelimitIdentifier ("ClassID"));
      }

      command.CommandText = string.Format (
          "UPDATE {0} SET {1} WHERE {2}{3}",
          Provider.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName()),
          updateSetBuilder,
          whereClauseBuilder,
          Provider.StatementDelimiter);

      return command;
    }

    protected virtual void AppendColumn (StringBuilder updateSetBuilder, string columnName, string parameterName)
    {
      if (updateSetBuilder.Length > 0)
        updateSetBuilder.Append (", ");

      updateSetBuilder.AppendFormat (
          "{0} = {1}",
          Provider.DelimitIdentifier (columnName),
          Provider.GetParameterName (parameterName));
    }

    protected void AddObjectIDAndClassIDParameters (
        StringBuilder updateSetBuilder,
        IDbCommand command,
        ClassDefinition classDefinition,
        PropertyValue propertyValue)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      ClassDefinition relatedClassDefinition;
      object relatedIDValue;
      if (propertyValue.GetFieldValue (ValueAccess.Current) != null)
      {
        var relatedID = (ObjectID) propertyValue.GetFieldValue (ValueAccess.Current);
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
        AddClassIDParameter (updateSetBuilder, command, relatedClassDefinition, propertyValue);
    }

    protected void AddClassIDParameter (
        StringBuilder updateSetBuilder,
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
        AppendColumn (updateSetBuilder, classIDColumnName, classIDColumnName);

        string classID = null;
        if (propertyValue.GetFieldValue (ValueAccess.Current) != null)
          classID = relatedClassDefinition.ID;

        AddCommandParameter (command, classIDColumnName, classID);
      }
    }

    private bool MustBeUpdated (PropertyValue propertyValue)
    {
      return (_dataContainer.State == StateType.New && propertyValue.PropertyType == typeof (ObjectID))
             || (_dataContainer.State == StateType.Deleted && propertyValue.PropertyType == typeof (ObjectID))
             || (_dataContainer.State == StateType.Changed && propertyValue.HasChanged);
    }

    private void AddPropertyValue (StringBuilder updateSetBuilder, IDbCommand command, PropertyValue propertyValue)
    {
      AppendColumn (updateSetBuilder, propertyValue.Definition.StorageSpecificName, propertyValue.Definition.StorageSpecificName);

      if (propertyValue.PropertyType != typeof (ObjectID))
        AddCommandParameter (command, propertyValue.Definition.StorageSpecificName, propertyValue);
      else
        AddObjectIDAndClassIDParameters (updateSetBuilder, command, _dataContainer.ClassDefinition, propertyValue);
    }
  }
}