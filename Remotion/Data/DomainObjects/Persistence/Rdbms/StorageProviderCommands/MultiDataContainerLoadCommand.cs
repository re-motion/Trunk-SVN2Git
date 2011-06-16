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
  public class MultiDataContainerLoadCommand : IStorageProviderCommand<IEnumerable<DataContainer>>
  {
    private readonly IDbCommandBuilder[] _dbCommandBuilders;
    private readonly bool _allowNulls;
    private readonly IDbCommandFactory _dbCommandFactory;
    private readonly IDbCommandExecutor _dbCommandExecutor;
    private readonly IDataContainerReader _dataContainerReader;

    public MultiDataContainerLoadCommand (
        IEnumerable<IDbCommandBuilder> dbCommandBuilders,
        bool allowNulls,
        IDbCommandFactory dbCommandFactory,
        IDbCommandExecutor dbCommandExecutor,
        IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilders", dbCommandBuilders);
      ArgumentUtility.CheckNotNull ("dbCommandFactory", dbCommandFactory);
      ArgumentUtility.CheckNotNull ("dbCommandExecutor", dbCommandExecutor);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      _dbCommandBuilders = dbCommandBuilders.ToArray();
      _allowNulls = allowNulls;
      _dbCommandFactory = dbCommandFactory;
      _dbCommandExecutor = dbCommandExecutor;
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

    public IDbCommandFactory DbCommandFactory
    {
      get { return _dbCommandFactory; }
    }

    public IDbCommandExecutor DbCommandExecutor
    {
      get { return _dbCommandExecutor; }
    }

    public IDataContainerReader DataContainerReader
    {
      get { return _dataContainerReader; }
    }

    public IEnumerable<DataContainer> Execute ()
    {
      return _dbCommandBuilders.SelectMany (builder => LoadDataContainersFromCommandBuilder (builder, _allowNulls)).ToArray();
    }

    private IEnumerable<DataContainer> LoadDataContainersFromCommandBuilder (IDbCommandBuilder commandBuilder, bool allowNulls)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      using (var command = commandBuilder.Create (_dbCommandFactory))
      {
        using (var reader = _dbCommandExecutor.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          return _dataContainerReader.ReadSequence (reader, allowNulls);
        }
      }
    }
  }
}