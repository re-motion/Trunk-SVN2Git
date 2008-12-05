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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
public class UpdateCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;
  private StringBuilder _updateBuilder;

  // construction and disposing

  public UpdateCommandBuilder (RdbmsProvider provider, DataContainer dataContainer) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    if (dataContainer.State == StateType.Unchanged)
      throw CreateArgumentException ("dataContainer", "State of provided DataContainer must not be 'Unchanged'.");

    _dataContainer = dataContainer;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = Provider.CreateDbCommand ();
    _updateBuilder = new StringBuilder ();

    foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
    {
      if (propertyValue.Definition.StorageClass == StorageClass.Persistent && MustBeUpdated (propertyValue))
        AddPropertyValue (command, propertyValue);
    }

    if (command.Parameters.Count == 0)
    {
      command.Dispose ();
      return null;
    }

    WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);
    whereClauseBuilder.Add ("ID", _dataContainer.ID.Value);

    if (_dataContainer.State != StateType.New)
      whereClauseBuilder.Add ("Timestamp", _dataContainer.Timestamp);

    command.CommandText = string.Format (
        "UPDATE {0} SET {1} WHERE {2}{3}",
        Provider.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName ()),
        _updateBuilder.ToString (), 
        whereClauseBuilder.ToString (),
        Provider.StatementDelimiter);

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    if (_updateBuilder.Length > 0)
      _updateBuilder.Append (", ");

    _updateBuilder.AppendFormat ("{0} = {1}", 
        Provider.DelimitIdentifier (columnName), 
        Provider.GetParameterName (parameterName));
  }

  private void AddPropertyValue (IDbCommand command, PropertyValue propertyValue)
  {
    AppendColumn (propertyValue.Definition.StorageSpecificName, propertyValue.Definition.StorageSpecificName);

    if (propertyValue.PropertyType != typeof (ObjectID))
      AddCommandParameter (command, propertyValue.Definition.StorageSpecificName, propertyValue);
    else
      AddObjectIDAndClassIDParameters (command, _dataContainer.ClassDefinition, propertyValue);
  }

  private bool MustBeUpdated (PropertyValue propertyValue)
  {
    return (_dataContainer.State == StateType.New && propertyValue.PropertyType == typeof (ObjectID)) 
        || (_dataContainer.State == StateType.Deleted && propertyValue.PropertyType == typeof (ObjectID)) 
        || (_dataContainer.State == StateType.Changed && propertyValue.HasChanged);
  }
}
}
