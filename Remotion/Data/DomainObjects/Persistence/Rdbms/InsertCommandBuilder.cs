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
  public class InsertCommandBuilder: CommandBuilder
  {
    // types

    // static members and constants

    // member fields

    private DataContainer _dataContainer;
    private StringBuilder _columnBuilder;
    private StringBuilder _valueBuilder;

    // construction and disposing

    public InsertCommandBuilder (RdbmsProvider provider, DataContainer dataContainer)
        : base (provider)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State != StateType.New)
        throw CreateArgumentException ("dataContainer", "State of provided DataContainer must be 'New', but is '{0}'.", dataContainer.State);

      _dataContainer = dataContainer;
    }

    // methods and properties

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();

      _columnBuilder = new StringBuilder();
      _valueBuilder = new StringBuilder();

      string idColumn = "ID";
      string classIDColumn = "ClassID";

      AppendColumn (idColumn, idColumn);
      AppendColumn (classIDColumn, classIDColumn);

      AddCommandParameter (command, idColumn, _dataContainer.ID);
      AddCommandParameter (command, classIDColumn, _dataContainer.ID.ClassID);

      foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
      {
        if (propertyValue.Definition.StorageClass == StorageClass.Persistent && propertyValue.PropertyType != typeof (ObjectID))
        {
          AppendColumn (propertyValue.Definition.StorageSpecificName, propertyValue.Definition.StorageSpecificName);
          AddCommandParameter (command, propertyValue.Definition.StorageSpecificName, propertyValue);
        }
      }

      command.CommandText = string.Format (
          "INSERT INTO {0} ({1}) VALUES ({2}){3}",
          Provider.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName()),
          _columnBuilder.ToString(),
          _valueBuilder.ToString(),
          Provider.StatementDelimiter);

      return command;
    }

    protected override void AppendColumn (string columnName, string parameterName)
    {
      if (_columnBuilder.Length > 0)
        _columnBuilder.Append (", ");

      _columnBuilder.Append (Provider.DelimitIdentifier (columnName));

      if (_valueBuilder.Length > 0)
        _valueBuilder.Append (", ");

      _valueBuilder.Append (Provider.GetParameterName (parameterName));
    }
  }
}
