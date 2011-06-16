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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public class UpdateDbCommandBuilder : DbCommandBuilder
  {
    private readonly DataContainer _dataContainer;
    private readonly IStorageNameProvider _storageNameProvider;

    public UpdateDbCommandBuilder (
        IStorageNameProvider storageNameProvider, DataContainer dataContainer, ISqlDialect sqlDialect, IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State == StateType.Unchanged)
        throw new ArgumentException ("State of provided DataContainer must not be 'Unchanged'.", "dataContainer");

      _storageNameProvider = storageNameProvider;
      _dataContainer = dataContainer;
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public override IDbCommand Create (IDbCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      IDbCommand command = commandFactory.CreateDbCommand();
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
      whereClauseBuilder.Add (StorageNameProvider.IDColumnName, _dataContainer.ID.Value);

      if (_dataContainer.State != StateType.New)
        whereClauseBuilder.Add (StorageNameProvider.TimestampColumnName, _dataContainer.Timestamp);

      if (updateSetBuilder.Length == 0)
      {
        // SET [ClassID] = [ClassID] => dummy set if no other property is set
        // This occurs whenever the Timestamp should be incremented even though no property was changed. Used, e.g., via DomainObject.MarkAsChanged.
        updateSetBuilder.AppendFormat (
            "{0} = {1}",
            SqlDialect.DelimitIdentifier (StorageNameProvider.ClassIDColumnName),
            SqlDialect.DelimitIdentifier (StorageNameProvider.ClassIDColumnName));
      }

      command.CommandText = string.Format (
          "UPDATE {0} SET {1} WHERE {2}{3}",
          SqlDialect.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName()),
          updateSetBuilder,
          whereClauseBuilder,
          SqlDialect.StatementDelimiter);

      return command;
    }

    protected virtual void AppendColumn (StringBuilder updateSetBuilder, string columnName, string parameterName)
    {
      if (updateSetBuilder.Length > 0)
        updateSetBuilder.Append (", ");

      updateSetBuilder.AppendFormat (
          "{0} = {1}",
          SqlDialect.DelimitIdentifier (columnName),
          SqlDialect.GetParameterName (parameterName));
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
      if (propertyValue.GetValueWithoutEvents (ValueAccess.Current) != null)
      {
        var relatedID = (ObjectID) propertyValue.GetValueWithoutEvents (ValueAccess.Current);
        relatedClassDefinition = relatedID.ClassDefinition;
        relatedIDValue = relatedID;
      }
      else
      {
        relatedClassDefinition = classDefinition.GetMandatoryOppositeClassDefinition (propertyValue.Name);
        relatedIDValue = null;
      }

      AddCommandParameter (command, propertyValue.Definition.StoragePropertyDefinition.Name, relatedIDValue);

      if (classDefinition.StorageEntityDefinition.StorageProviderDefinition
          == relatedClassDefinition.StorageEntityDefinition.StorageProviderDefinition)
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
        string classIDColumnName = StorageNameProvider.GetRelationClassIDColumnName (propertyValue.Definition);
        AppendColumn (updateSetBuilder, classIDColumnName, classIDColumnName);

        string classID = null;
        if (propertyValue.GetValueWithoutEvents (ValueAccess.Current) != null)
          classID = relatedClassDefinition.ID;

        AddCommandParameter (command, classIDColumnName, classID);
      }
    }

    private bool MustBeUpdated (PropertyValue propertyValue)
    {
      return (_dataContainer.State == StateType.New && propertyValue.Definition.IsObjectID)
             || (_dataContainer.State == StateType.Deleted && propertyValue.Definition.IsObjectID)
             || (_dataContainer.State == StateType.Changed && propertyValue.HasChanged);
    }

    private void AddPropertyValue (StringBuilder updateSetBuilder, IDbCommand command, PropertyValue propertyValue)
    {
      AppendColumn (
          updateSetBuilder, propertyValue.Definition.StoragePropertyDefinition.Name, propertyValue.Definition.StoragePropertyDefinition.Name);

      if (!propertyValue.Definition.IsObjectID)
        AddCommandParameter (command, propertyValue.Definition.StoragePropertyDefinition.Name, propertyValue);
      else
        AddObjectIDAndClassIDParameters (updateSetBuilder, command, _dataContainer.ClassDefinition, propertyValue);
    }
  }
}