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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public class DeleteDbCommandBuilder : DbCommandBuilder
  {
    // types

    // static members and constants

    // member fields

    private readonly DataContainer _dataContainer;

    // construction and disposing

    public DeleteDbCommandBuilder (
        RdbmsProvider provider,
        IStorageNameProvider storageNameProvider,
        DataContainer dataContainer,
        ISqlDialect sqlDialect,
        IDbCommandFactory commandFactory)
        : base (provider, storageNameProvider, sqlDialect, commandFactory)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State != StateType.Deleted)
        throw CreateArgumentException ("dataContainer", "State of provided DataContainer must be 'Deleted', but is '{0}'.", dataContainer.State);

      _dataContainer = dataContainer;
    }

    // methods and properties

    public override IDbCommand Create ()
    {
      IDbCommand command = CommandFactory.CreateDbCommand();

      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);
      whereClauseBuilder.Add (StorageNameProvider.IDColumnName, _dataContainer.ID.Value);

      if (MustAddTimestampToWhereClause())
        whereClauseBuilder.Add (StorageNameProvider.TimestampColumnName, _dataContainer.Timestamp);

      command.CommandText = string.Format (
          "DELETE FROM {0} WHERE {1}{2}",
          SqlDialect.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName()),
          whereClauseBuilder,
          SqlDialect.StatementDelimiter);

      return command;
    }

    private bool MustAddTimestampToWhereClause ()
    {
      foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
      {
        if (propertyValue.Definition.IsObjectID)
          return false;
      }

      return true;
    }
  }
}