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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a single <see cref="DataContainer"/>.
  /// </summary>
  public class SingleDataContainerLoadCommand : IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext>
  {
    private readonly IDbCommandBuilder _dbCommandBuilder;
    private readonly IDataContainerReader _dataContainerReader;
    private readonly IRdbmsProviderCommandExecutionContext _commandExecutionContext;

    // TODO Review 4068: Remove IRdbmsProviderCommandExecutionContext parameter (here and for all commands)
    public SingleDataContainerLoadCommand (
        IDbCommandBuilder dbCommandBuilder,
        IRdbmsProviderCommandExecutionContext commandExecutionContext,
        IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilder", dbCommandBuilder);
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      _dbCommandBuilder = dbCommandBuilder;
      _commandExecutionContext = commandExecutionContext;
      _dataContainerReader = dataContainerReader;
    }

    public IDbCommandBuilder DbCommandBuilder
    {
      get { return _dbCommandBuilder; }
    }

    public IRdbmsProviderCommandExecutionContext CommandExecutionContext
    {
      get { return _commandExecutionContext; }
    }

    public IDataContainerReader DataContainerReader
    {
      get { return _dataContainerReader; }
    }

    public DataContainer Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      using (var command = _dbCommandBuilder.Create (_commandExecutionContext))
      {
        using (var reader = _commandExecutionContext.ExecuteReader (command, CommandBehavior.SingleRow))
        {
          return _dataContainerReader.Read(reader);
        }
      }
    }
  }
}