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
        if (propertyValue.PropertyType != typeof (ObjectID))
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
