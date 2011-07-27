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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public class LegacyDeleteDbCommandBuilder : DbCommandBuilder
  {
    private readonly DataContainer _dataContainer;
    private readonly IStorageNameProvider _storageNameProvider;

    public LegacyDeleteDbCommandBuilder (
        IStorageNameProvider storageNameProvider, DataContainer dataContainer, ISqlDialect sqlDialect, IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State != StateType.Deleted)
      {
        throw new ArgumentException (
            string.Format ("State of provided DataContainer must be 'Deleted', but is '{0}'.", dataContainer.State), "dataContainer");
      }

      _storageNameProvider = storageNameProvider;
      _dataContainer = dataContainer;
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      IDbCommand command = commandExecutionContext.CreateDbCommand ();

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