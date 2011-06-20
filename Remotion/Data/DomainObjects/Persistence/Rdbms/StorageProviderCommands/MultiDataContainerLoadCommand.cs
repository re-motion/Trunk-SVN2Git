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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a sequence of <see cref="DataContainer"/>
  /// instances.
  /// </summary>
  public class MultiDataContainerLoadCommand : IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>
  {
    private readonly IDbCommandBuilder[] _dbCommandBuilders;
    private readonly bool _allowNulls;
    private readonly IRdbmsProviderCommandExecutionContext _commandExecutionContext;
    private readonly IDataContainerReader _dataContainerReader;

    public MultiDataContainerLoadCommand (
        IEnumerable<IDbCommandBuilder> dbCommandBuilders,
        bool allowNulls,
        IRdbmsProviderCommandExecutionContext commandExecutionContext,
        IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilders", dbCommandBuilders);
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      _dbCommandBuilders = dbCommandBuilders.ToArray();
      _allowNulls = allowNulls;
      _commandExecutionContext = commandExecutionContext;
      _dataContainerReader = dataContainerReader;
    }

    public IDbCommandBuilder[] DbCommandBuilders
    {
      get { return _dbCommandBuilders; }
    }

    public bool AllowNulls
    {
      get { return _allowNulls; }
    }

    public IRdbmsProviderCommandExecutionContext CommandExecutionContext
    {
      get { return _commandExecutionContext; }
    }

    public IDataContainerReader DataContainerReader
    {
      get { return _dataContainerReader; }
    }

    public IEnumerable<DataContainer> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);
      return _dbCommandBuilders.SelectMany (LoadDataContainersFromCommandBuilder).ToArray();
    }

    private IEnumerable<DataContainer> LoadDataContainersFromCommandBuilder (IDbCommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      using (var command = commandBuilder.Create (_commandExecutionContext))
      {
        using (var reader = _commandExecutionContext.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          return _dataContainerReader.ReadSequence (reader, _allowNulls);
        }
      }
    }
  }
}