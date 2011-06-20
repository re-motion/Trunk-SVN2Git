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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a sequence of <see cref="ObjectID"/>
  /// instances.
  /// </summary>
  public class MultiObjectIDLoadCommand : IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext>
  {
    private readonly IEnumerable<IDbCommandBuilder> _dbCommandBuilders;
    private readonly IObjectIDFactory _objectIDFactory;
    private readonly IRdbmsProviderCommandExecutionContext _commandExecutionReader;

    public MultiObjectIDLoadCommand (
        IEnumerable<IDbCommandBuilder> dbCommandBuilders,
        IRdbmsProviderCommandExecutionContext commandExecutionContext,
        IObjectIDFactory objectIDFactory)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilders", dbCommandBuilders);
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);
      ArgumentUtility.CheckNotNull ("objectIDFactory", objectIDFactory);

      _dbCommandBuilders = dbCommandBuilders;
      _commandExecutionReader = commandExecutionContext;
      _objectIDFactory = objectIDFactory;
    }

    public IEnumerable<IDbCommandBuilder> DbCommandBuilders
    {
      get { return _dbCommandBuilders; }
    }

    public IRdbmsProviderCommandExecutionContext CommandExecutionContext
    {
      get { return _commandExecutionReader; }
    }

    public IObjectIDFactory ObjectIDFactory
    {
      get { return _objectIDFactory; }
    }

    public IEnumerable<ObjectID> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);
      return _dbCommandBuilders.SelectMany (LoadObjectIDsFromCommandBuilder);
    }

    private IEnumerable<ObjectID> LoadObjectIDsFromCommandBuilder (IDbCommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      using (var command = commandBuilder.Create (_commandExecutionReader))
      {
        using (var reader = _commandExecutionReader.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          return _objectIDFactory.ReadSequence (reader);
        }
      }
    }
  }
}