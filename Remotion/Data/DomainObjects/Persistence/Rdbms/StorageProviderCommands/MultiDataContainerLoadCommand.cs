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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a sequence of <see cref="DataContainer"/>
  /// instances.
  /// </summary>
  public class MultiDataContainerLoadCommand : IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>
  {
    private readonly Tuple<IDbCommandBuilder, IDataContainerReader>[] _dbCommandBuildersAndReaders;

    public MultiDataContainerLoadCommand (IEnumerable<Tuple<IDbCommandBuilder, IDataContainerReader>> dbCommandBuildersAndReaders)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuildersAndReaders", dbCommandBuildersAndReaders);

      _dbCommandBuildersAndReaders = dbCommandBuildersAndReaders.ToArray();
    }

    public Tuple<IDbCommandBuilder, IDataContainerReader>[] DbCommandBuildersAndReaders
    {
      get { return _dbCommandBuildersAndReaders; }
    }


    public IEnumerable<DataContainer> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);
      return _dbCommandBuildersAndReaders.SelectMany (b => LoadDataContainersFromCommandBuilder (b, executionContext)).ToArray();
    }

    private IEnumerable<DataContainer> LoadDataContainersFromCommandBuilder (
        Tuple<IDbCommandBuilder, IDataContainerReader> commandBuilderTuple, IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("commandBuilderTuple", commandBuilderTuple);

      using (var command = commandBuilderTuple.Item1.Create (executionContext))
      {
        using (var reader = executionContext.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          foreach (var dataContainer in commandBuilderTuple.Item2.ReadSequence (reader))
            yield return dataContainer;
        }
      }
    }
  }
}